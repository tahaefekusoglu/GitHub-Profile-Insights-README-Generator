using System.Text;
using System.Text.Json;
using GitHubReadmeGenerator.API.Models;

namespace GitHubReadmeGenerator.API.Services;

public class GeminiService : IAiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string? _apiKey;

    public GeminiService(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = config["Gemini:ApiKey"]
                  ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
    }

    public async Task<string> GenerateBioAsync(GitHubProfile profile)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("Gemini API key is not configured");

        var text = await GenerateTextAsync(BuildBioPrompt(profile));
        if (string.IsNullOrWhiteSpace(text))
            throw new Exception("Empty response from Gemini");

        return text.Trim();
    }

    public async Task<ProfileAnalysis> GenerateAnalysisAsync(GitHubProfile profile)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("Gemini API key is not configured");

        var text = await GenerateTextAsync(BuildAnalysisPrompt(profile));
        if (string.IsNullOrWhiteSpace(text))
            throw new Exception("Empty response from Gemini");

        var json = text.Trim();
        var jsonStart = json.IndexOf('{');
        var jsonEnd = json.LastIndexOf('}');
        if (jsonStart >= 0 && jsonEnd > jsonStart)
            json = json[jsonStart..(jsonEnd + 1)];

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var raw = JsonSerializer.Deserialize<RawAnalysis>(json, opts)
                  ?? throw new Exception("Failed to parse analysis JSON");
        var yearsOnGitHub = DateTime.UtcNow.Year - profile.CreatedAt.Year;

        return new ProfileAnalysis
        {
            Username = profile.Login,
            DeveloperType = raw.DeveloperType ?? "",
            ExperienceLevel = raw.ExperienceLevel ?? "",
            PrimaryFocus = raw.PrimaryFocus ?? "",
            Strengths = raw.Strengths ?? new(),
            Insights = raw.Insights ?? new(),
            TechStack = raw.TechStack ?? new(),
            Summary = raw.Summary ?? "",
            IsAiGenerated = true,
            AiProvider = "gemini",
            Stats = new AnalysisStats
            {
                PublicRepos = profile.PublicRepos,
                TotalStars = profile.TotalStars,
                Followers = profile.Followers,
                Following = profile.Following,
                YearsOnGitHub = yearsOnGitHub,
                AvatarUrl = profile.AvatarUrl,
                Name = profile.Name,
                Bio = profile.Bio,
                Location = profile.Location,
                Company = profile.Company,
                Blog = profile.Blog,
                Languages = profile.TopLanguages,
                TopRepos = profile.TopRepos
            }
        };
    }

    private async Task<string> GenerateTextAsync(string prompt)
    {
        var client = _httpClientFactory.CreateClient();
        var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";
        var request = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                maxOutputTokens = 800
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(endpoint, content);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Gemini API error: {response.StatusCode} {body}");

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "";
    }

    private static string BuildBioPrompt(GitHubProfile profile)
    {
        var languages = profile.TopLanguages.Count > 0
            ? string.Join(", ", profile.TopLanguages.Select(l => l.Language))
            : "not specified";

        var repos = profile.TopRepos.Take(3)
            .Select(r => r.Description != null ? $"{r.Name}: {r.Description}" : r.Name);

        return $"""
You are writing a GitHub profile bio for a developer.

Here is their data:
- Name: {profile.Name}
- GitHub Bio: {profile.Bio ?? "not provided"}
- Location: {profile.Location ?? "not provided"}
- Company: {profile.Company ?? "not provided"}
- Public Repos: {profile.PublicRepos}
- Total Stars: {profile.TotalStars}
- Followers: {profile.Followers}
- Member since: {profile.CreatedAt.Year}
- Top Languages: {languages}
- Notable repos: {string.Join(", ", repos)}

Write a 2-3 sentence, first-person GitHub profile bio. It should be professional, specific to their actual work, and highlight their technical strengths. Do not use hashtags, emojis, or generic phrases like "passionate developer". Return only the bio text, nothing else.
""";
    }

    private static string BuildAnalysisPrompt(GitHubProfile profile)
    {
        var languages = profile.TopLanguages.Count > 0
            ? string.Join(", ", profile.TopLanguages.Select(l => $"{l.Language} ({l.Percentage}%)"))
            : "not specified";

        var repos = profile.TopRepos.Take(5)
            .Select(r => r.Description != null
                ? $"{r.Name} ({r.Stars} stars): {r.Description}"
                : $"{r.Name} ({r.Stars} stars)");

        var yearsOnGitHub = DateTime.UtcNow.Year - profile.CreatedAt.Year;

        var jsonSchema = """
{
  "developerType": "e.g. Full-Stack Developer, Backend Engineer, Data Scientist, DevOps Engineer",
  "experienceLevel": "one of: Junior, Mid-level, Senior, Expert",
  "primaryFocus": "1-2 sentences describing what this developer primarily works on",
  "strengths": ["strength 1", "strength 2", "strength 3", "strength 4"],
  "insights": ["interesting observation 1", "interesting observation 2", "interesting observation 3"],
  "techStack": ["Technology1", "Technology2", "Technology3", "Technology4", "Technology5"],
  "summary": "2-3 sentence overall assessment of this developer's profile, skills, and contributions"
}
""";

        return $"""
You are an expert developer analyst. Analyze this GitHub profile and return a JSON object.

Profile data:
- Username: {profile.Login}
- Name: {profile.Name}
- Bio: {profile.Bio ?? "not provided"}
- Location: {profile.Location ?? "not provided"}
- Company: {profile.Company ?? "not provided"}
- Public Repos: {profile.PublicRepos}
- Total Stars: {profile.TotalStars}
- Followers: {profile.Followers}
- Following: {profile.Following}
- Years on GitHub: {yearsOnGitHub}
- Top Languages: {languages}
- Notable repos: {string.Join(" | ", repos)}

Return ONLY a valid JSON object with exactly these fields (no markdown, no extra text):
{jsonSchema}
Base your analysis on actual data. Be specific, not generic. If data is limited, note that in insights.
""";
    }

    private class RawAnalysis
    {
        public string? DeveloperType { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? PrimaryFocus { get; set; }
        public List<string>? Strengths { get; set; }
        public List<string>? Insights { get; set; }
        public List<string>? TechStack { get; set; }
        public string? Summary { get; set; }
    }
}
