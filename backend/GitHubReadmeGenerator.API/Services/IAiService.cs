using GitHubReadmeGenerator.API.Models;

namespace GitHubReadmeGenerator.API.Services;

public interface IAiService
{
    Task<string> GenerateBioAsync(GitHubProfile profile);
    Task<ProfileAnalysis> GenerateAnalysisAsync(GitHubProfile profile);
}
