using GitHubReadmeGenerator.API.Models;

namespace GitHubReadmeGenerator.API.Services;

public static class LocalProfileAnalyzer
{
    public static ProfileAnalysis Analyze(GitHubProfile profile)
    {
        var yearsOnGitHub = Math.Max(0, DateTime.UtcNow.Year - profile.CreatedAt.Year);
        var langs = profile.TopLanguages.Select(l => l.Language.ToLower()).ToList();
        var topLang = profile.TopLanguages.FirstOrDefault()?.Language ?? "";
        var avgStars = profile.PublicRepos > 0
            ? (double)profile.TotalStars / profile.PublicRepos
            : 0;

        var developerType = DetectDeveloperType(langs);
        var experienceLevel = DetectExperienceLevel(profile, yearsOnGitHub);

        return new ProfileAnalysis
        {
            Username = profile.Login,
            DeveloperType = developerType,
            ExperienceLevel = experienceLevel,
            PrimaryFocus = BuildPrimaryFocus(profile, developerType, topLang, langs),
            Strengths = BuildStrengths(profile, langs, yearsOnGitHub, avgStars),
            Insights = BuildInsights(profile, langs, yearsOnGitHub, avgStars, topLang),
            TechStack = profile.TopLanguages.Select(l => l.Language).Take(6).ToList(),
            Summary = BuildSummary(profile, developerType, experienceLevel, yearsOnGitHub, topLang),
            IsAiGenerated = false,
            AiProvider = null,
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

    // ── Developer Type ────────────────────────────────────────────────────────

    private static string DetectDeveloperType(List<string> langs)
    {
        bool has(params string[] kw) => kw.Any(k => langs.Contains(k));

        if (has("swift", "objective-c") && !has("kotlin", "java")) return "iOS Developer";
        if (has("dart")) return "Flutter / Mobile Developer";
        if (has("kotlin") && langs.IndexOf("kotlin") == 0) return "Android Developer";

        if (has("r", "julia", "matlab")) return "Data Scientist";

        bool pyFirst = langs.FirstOrDefault() == "python";
        if (pyFirst && !has("javascript", "typescript", "html", "css"))
            return "Python / Data Developer";

        if ((has("rust", "zig") || (has("c", "c++") && !has("javascript", "typescript", "python", "java"))))
            return "Systems Engineer";

        if (has("shell", "hcl", "dockerfile", "makefile") &&
            !has("javascript", "typescript", "python", "java", "c#"))
            return "DevOps Engineer";

        if (has("c#") && !has("javascript", "typescript", "python"))
            return ".NET Developer";

        if ((has("java") || has("kotlin", "scala")) &&
            !has("javascript", "typescript", "python", "c#"))
            return "JVM Developer";

        bool frontend = has("javascript", "typescript", "html", "css", "vue", "svelte", "elm");
        bool backend = has("python", "java", "go", "c#", "php", "ruby", "rust", "scala", "elixir");

        if (frontend && backend) return "Full-Stack Developer";
        if (frontend) return "Frontend Developer";
        if (backend) return "Backend Developer";

        return "Software Developer";
    }

    // ── Experience Level ──────────────────────────────────────────────────────

    private static string DetectExperienceLevel(GitHubProfile profile, int years)
    {
        int score = 0;

        // Years
        if (years >= 10) score += 4;
        else if (years >= 7) score += 3;
        else if (years >= 4) score += 2;
        else if (years >= 2) score += 1;

        // Stars
        if (profile.TotalStars >= 1000) score += 4;
        else if (profile.TotalStars >= 300) score += 3;
        else if (profile.TotalStars >= 50) score += 2;
        else if (profile.TotalStars >= 10) score += 1;

        // Repos
        if (profile.PublicRepos >= 80) score += 3;
        else if (profile.PublicRepos >= 40) score += 2;
        else if (profile.PublicRepos >= 15) score += 1;

        // Followers
        if (profile.Followers >= 500) score += 3;
        else if (profile.Followers >= 100) score += 2;
        else if (profile.Followers >= 20) score += 1;

        return score switch
        {
            >= 10 => "Expert",
            >= 6  => "Senior",
            >= 3  => "Mid-level",
            _     => "Junior"
        };
    }

    // ── Primary Focus ─────────────────────────────────────────────────────────

    private static string BuildPrimaryFocus(
        GitHubProfile profile, string devType, string topLang, List<string> langs)
    {
        var name = profile.Name ?? profile.Login;

        if (profile.PublicRepos == 0)
            return $"{name} is new to GitHub and hasn't published public repositories yet.";

        var topRepoDesc = profile.TopRepos.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Description));
        var repoHint = topRepoDesc is not null ? $" Their most notable project is \"{topRepoDesc.Name}\"." : "";

        var langLine = string.IsNullOrWhiteSpace(topLang)
            ? ""
            : $" They primarily work with {topLang}" +
              (langs.Count > 1 ? $" alongside {string.Join(", ", langs.Skip(1).Take(2).Select(Capitalize))}." : ".");

        return $"{name} is a {devType} focused on building software with a public portfolio of {profile.PublicRepos} repositories.{langLine}{repoHint}";
    }

    // ── Strengths ─────────────────────────────────────────────────────────────

    private static List<string> BuildStrengths(
        GitHubProfile profile, List<string> langs, int years, double avgStars)
    {
        var list = new List<string>();

        if (profile.TopLanguages.Count >= 4)
            list.Add($"Polyglot developer — works across {profile.TopLanguages.Count} languages");

        if (avgStars >= 20)
            list.Add("Writes high-quality, widely-appreciated open source code");
        else if (profile.TotalStars >= 50)
            list.Add("Has built projects that resonate with the open source community");

        if (years >= 6)
            list.Add($"Over {years} years of continuous GitHub presence");
        else if (years >= 3)
            list.Add("Consistent long-term contributor to open source");

        if (profile.Followers >= 200)
            list.Add($"Recognized community figure with {profile.Followers} followers");
        else if (profile.Followers >= 50)
            list.Add("Growing community recognition and following");

        if (profile.PublicRepos >= 40)
            list.Add($"Prolific contributor with {profile.PublicRepos} public repositories");

        // Language-specific strengths
        if (langs.Contains("typescript"))
            list.Add("Emphasizes type safety and scalable code with TypeScript");
        if (langs.Contains("rust"))
            list.Add("Works with Rust — strong focus on performance and safety");
        if (langs.Contains("go"))
            list.Add("Go expertise suggests focus on concurrent, high-performance systems");
        if (langs.Contains("python") && langs.Contains("javascript"))
            list.Add("Bridges data/backend (Python) and frontend (JavaScript) worlds");

        if (profile.TotalStars == 0 && profile.PublicRepos > 0)
            list.Add("Active in building and sharing projects publicly");

        // Ensure at least 3
        if (list.Count < 3)
            list.Add("Maintains an active public GitHub profile");
        if (list.Count < 3)
            list.Add($"Primary language expertise in {Capitalize(langs.FirstOrDefault() ?? "software development")}");

        return list.Take(5).ToList();
    }

    // ── Insights ──────────────────────────────────────────────────────────────

    private static List<string> BuildInsights(
        GitHubProfile profile, List<string> langs, int years, double avgStars, string topLang)
    {
        var list = new List<string>();

        // Follower / following ratio
        if (profile.Following > 0)
        {
            var ratio = (double)profile.Followers / profile.Following;
            if (ratio >= 3)
                list.Add($"High follower-to-following ratio ({profile.Followers}:{profile.Following}) — a well-known profile in their community");
            else if (ratio < 0.3 && profile.Following > 20)
                list.Add("Follows many developers — likely an active community member and learner");
        }

        // Stars per repo
        if (avgStars >= 50)
            list.Add($"Exceptional average of {avgStars:F0} stars per repository — consistently creates popular projects");
        else if (avgStars >= 10)
            list.Add($"Averages {avgStars:F0} stars per repository, indicating well-received work");
        else if (profile.PublicRepos > 10 && profile.TotalStars < 10)
            list.Add("Many repositories but few stars — likely focuses on personal learning or private clients");

        // Account age insights
        if (years >= 10)
            list.Add($"A GitHub veteran with {years} years on the platform — has witnessed the platform grow");
        else if (years <= 1)
            list.Add("Relatively new to GitHub — still building their public portfolio");

        // Language diversity
        if (profile.TopLanguages.Count == 1)
            list.Add($"Highly specialized in {topLang} — deep expertise in a single language");
        else if (profile.TopLanguages.Count >= 5)
            list.Add("Works across many languages, suggesting broad adaptability or diverse project types");

        // Top repo
        var topRepo = profile.TopRepos.FirstOrDefault();
        if (topRepo is not null && topRepo.Stars >= 50)
            list.Add($"Most starred project \"{topRepo.Name}\" has {topRepo.Stars} stars — a meaningful open source contribution");

        // Minimal public activity
        if (profile.PublicRepos == 0)
            list.Add("No public repositories yet — may be working primarily in private repos or just getting started");

        if (list.Count < 3)
            list.Add($"Active in {Capitalize(topLang)} development based on their public repository history");
        if (list.Count < 3)
            list.Add("Profile data is limited — more analysis would require access to private activity");

        return list.Take(4).ToList();
    }

    // ── Summary ───────────────────────────────────────────────────────────────

    private static string BuildSummary(
        GitHubProfile profile, string devType, string level, int years, string topLang)
    {
        var name = profile.Name ?? profile.Login;
        var langPart = string.IsNullOrWhiteSpace(topLang) ? "" : $" with a focus on {topLang}";
        var yearPart = years > 0 ? $" with {years} years of GitHub history" : "";
        var starPart = profile.TotalStars > 0 ? $", having earned {profile.TotalStars} total stars across their repositories" : "";
        var followerPart = profile.Followers > 0 ? $" {profile.Followers} developers follow their work" : "";

        var sentence1 = $"{name} is a {level.ToLower()} {devType}{langPart}{yearPart}.";
        var sentence2 = $"They have {profile.PublicRepos} public repositories{starPart}.";
        var sentence3 = followerPart.Length > 0
            ? $"{followerPart}, reflecting their presence in the developer community."
            : "Their profile shows consistent engagement with open source software.";

        return $"{sentence1} {sentence2} {sentence3}";
    }

    private static string Capitalize(string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s[1..];
}
