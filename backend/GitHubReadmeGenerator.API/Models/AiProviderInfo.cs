namespace GitHubReadmeGenerator.API.Models;

public class AiModelInfo
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public bool IsDefault { get; set; }
}

public class AiProviderInfo
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public List<AiModelInfo> Models { get; set; } = new();
}

public class AvailableProvidersResponse
{
    public List<AiProviderInfo> Available { get; set; } = new();
}
