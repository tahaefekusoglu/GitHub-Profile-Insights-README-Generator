namespace GitHubReadmeGenerator.API.Models;

public class ReadmeConfig
{
    public string Username { get; set; } = "";
    public GitHubProfile Profile { get; set; } = new();
    public string Theme { get; set; } = "colorful";
    public List<string> EnabledSections { get; set; } = new();
    public string? AiBio { get; set; }
}
