using GitHubReadmeGenerator.API.Models;

namespace GitHubReadmeGenerator.API.Services;

public interface IGitHubService
{
    Task<GitHubProfile?> GetProfileAsync(string username);
}
