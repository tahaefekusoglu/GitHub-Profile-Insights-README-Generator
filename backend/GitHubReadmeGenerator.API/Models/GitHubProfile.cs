using System.Text.Json.Serialization;

namespace GitHubReadmeGenerator.API.Models;

public class GitHubProfile
{
    public string Login { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Bio { get; set; }
    public string AvatarUrl { get; set; } = "";
    public string? Location { get; set; }
    public string? Company { get; set; }
    public string? Blog { get; set; }
    public int PublicRepos { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    public int TotalStars { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<GitHubRepo> TopRepos { get; set; } = new();
    public List<LanguageStat> TopLanguages { get; set; } = new();
}
