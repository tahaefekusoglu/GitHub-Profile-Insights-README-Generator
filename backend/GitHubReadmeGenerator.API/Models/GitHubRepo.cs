namespace GitHubReadmeGenerator.API.Models;

public class GitHubRepo
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string HtmlUrl { get; set; } = "";
    public int Stars { get; set; }
    public int Forks { get; set; }
    public string? Language { get; set; }
}
