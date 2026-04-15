using GitHubReadmeGenerator.API.Models;

namespace GitHubReadmeGenerator.API.Services;

public interface IAiService
{
    Task<string> GenerateBioAsync(GitHubProfile profile, string? model = null);
    Task<ProfileAnalysis> GenerateAnalysisAsync(GitHubProfile profile, string? model = null);
}
