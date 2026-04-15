using GitHubReadmeGenerator.API.Models;

namespace GitHubReadmeGenerator.API.Services;

public interface IReadmeTemplateService
{
    string Generate(ReadmeConfig config);
}
