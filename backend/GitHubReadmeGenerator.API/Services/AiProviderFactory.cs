namespace GitHubReadmeGenerator.API.Services;

public static class AiProviderFactory
{
    public static string? GetActiveProvider(IConfiguration config)
    {
        if (!string.IsNullOrWhiteSpace(config["Anthropic:ApiKey"]
            ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")))
            return "claude";

        if (!string.IsNullOrWhiteSpace(config["OpenAI:ApiKey"]
            ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")))
            return "openai";

        if (!string.IsNullOrWhiteSpace(config["Gemini:ApiKey"]
            ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")))
            return "gemini";

        return null;
    }
}
