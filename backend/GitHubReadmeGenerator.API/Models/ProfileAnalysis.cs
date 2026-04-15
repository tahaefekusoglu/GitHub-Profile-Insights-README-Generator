namespace GitHubReadmeGenerator.API.Models;

public class ProfileAnalysis
{
    public string Username { get; set; } = "";
    public string DeveloperType { get; set; } = "";
    public string ExperienceLevel { get; set; } = "";
    public string PrimaryFocus { get; set; } = "";
    public List<string> Strengths { get; set; } = new();
    public List<string> Insights { get; set; } = new();
    public List<string> TechStack { get; set; } = new();
    public string Summary { get; set; } = "";
    public bool IsAiGenerated { get; set; }
    public string? AiProvider { get; set; }
    public AnalysisStats Stats { get; set; } = new();
}

public class AnalysisStats
{
    public int PublicRepos { get; set; }
    public int TotalStars { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    public int YearsOnGitHub { get; set; }
    public string AvatarUrl { get; set; } = "";
    public string? Name { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? Company { get; set; }
    public string? Blog { get; set; }
    public List<LanguageStat> Languages { get; set; } = new();
    public List<GitHubRepo> TopRepos { get; set; } = new();
}
