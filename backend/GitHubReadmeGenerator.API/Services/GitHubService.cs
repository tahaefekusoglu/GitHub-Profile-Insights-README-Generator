using System.Text.Json;
using System.Text.Json.Serialization;
using GitHubReadmeGenerator.API.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GitHubReadmeGenerator.API.Services;

public class GitHubService : IGitHubService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly IMemoryCache _cache;

    public GitHubService(IHttpClientFactory httpClientFactory, IConfiguration config, IMemoryCache cache)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _cache = cache;
    }

    private static readonly object _notFoundSentinel = new();

    public async Task<GitHubProfile?> GetProfileAsync(string username)
    {
        var cacheKey = $"github_{username.ToLower()}";
        if (_cache.TryGetValue(cacheKey, out object? cachedObj))
        {
            if (ReferenceEquals(cachedObj, _notFoundSentinel)) return null;
            return cachedObj as GitHubProfile;
        }

        var client = _httpClientFactory.CreateClient("github");

        var token = _config["GitHub:Token"]
                    ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (!string.IsNullOrWhiteSpace(token) &&
            !client.DefaultRequestHeaders.Contains("Authorization"))
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var userResponse = await client.GetAsync($"users/{username}");
        if (userResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _cache.Set(cacheKey, _notFoundSentinel, TimeSpan.FromMinutes(2));
            return null;
        }
        if (!userResponse.IsSuccessStatusCode)
        {
            var body = await userResponse.Content.ReadAsStringAsync();
            if (userResponse.StatusCode == System.Net.HttpStatusCode.Forbidden &&
                body.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
                throw new HttpRequestException("GitHub rate limit exceeded");

            throw new HttpRequestException($"GitHub API error: {userResponse.StatusCode}");
        }

        var userJson = await userResponse.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var raw = JsonSerializer.Deserialize<RawGitHubUser>(userJson, options)!;

        var reposResponse = await client.GetAsync($"users/{username}/repos?sort=stars&per_page=100");
        reposResponse.EnsureSuccessStatusCode();
        var reposJson = await reposResponse.Content.ReadAsStringAsync();
        var rawRepos = JsonSerializer.Deserialize<List<RawGitHubRepo>>(reposJson, options) ?? new();

        var totalStars = rawRepos.Sum(r => r.StargazersCount);
        var topRepos = rawRepos
            .OrderByDescending(r => r.StargazersCount)
            .Take(6)
            .Select(r => new GitHubRepo
            {
                Name = r.Name,
                Description = r.Description,
                HtmlUrl = r.HtmlUrl,
                Stars = r.StargazersCount,
                Forks = r.ForksCount,
                Language = r.Language
            })
            .ToList();

        var langCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var repo in rawRepos)
        {
            if (!string.IsNullOrWhiteSpace(repo.Language))
            {
                langCounts.TryGetValue(repo.Language, out var count);
                langCounts[repo.Language] = count + 1;
            }
        }

        var total = langCounts.Values.Sum();
        var topLanguages = langCounts
            .OrderByDescending(kv => kv.Value)
            .Take(5)
            .Select(kv => new LanguageStat
            {
                Language = kv.Key,
                Percentage = total > 0 ? Math.Round((double)kv.Value / total * 100, 1) : 0
            })
            .ToList();

        var profile = new GitHubProfile
        {
            Login = raw.Login,
            Name = raw.Name ?? raw.Login,
            Bio = raw.Bio,
            AvatarUrl = raw.AvatarUrl,
            Location = raw.Location,
            Company = raw.Company,
            Blog = string.IsNullOrWhiteSpace(raw.Blog) ? null : raw.Blog,
            PublicRepos = raw.PublicRepos,
            Followers = raw.Followers,
            Following = raw.Following,
            TotalStars = totalStars,
            CreatedAt = raw.CreatedAt,
            TopRepos = topRepos,
            TopLanguages = topLanguages
        };

        _cache.Set(cacheKey, (object)profile, TimeSpan.FromMinutes(5));
        return profile;
    }

    private class RawGitHubUser
    {
        public string Login { get; set; } = "";
        public string? Name { get; set; }
        public string? Bio { get; set; }
        [JsonPropertyName("avatar_url")] public string AvatarUrl { get; set; } = "";
        public string? Location { get; set; }
        public string? Company { get; set; }
        public string? Blog { get; set; }
        [JsonPropertyName("public_repos")] public int PublicRepos { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
    }

    private class RawGitHubRepo
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = "";
        [JsonPropertyName("stargazers_count")] public int StargazersCount { get; set; }
        [JsonPropertyName("forks_count")] public int ForksCount { get; set; }
        public string? Language { get; set; }
    }
}
