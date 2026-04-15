using GitHubReadmeGenerator.API.Models;

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

    public static AvailableProvidersResponse GetAvailableProviders(IConfiguration config)
    {
        var list = new List<AiProviderInfo>();

        if (!string.IsNullOrWhiteSpace(config["Anthropic:ApiKey"]
            ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")))
        {
            list.Add(new AiProviderInfo
            {
                Id = "claude",
                Name = "Claude (Anthropic)",
                Models = new List<AiModelInfo>
                {
                    new() { Id = "claude-3-5-sonnet-20241022", Name = "Claude 3.5 Sonnet", IsDefault = true },
                    new() { Id = "claude-3-5-haiku-20241022",  Name = "Claude 3.5 Haiku" },
                    new() { Id = "claude-3-opus-20240229",      Name = "Claude 3 Opus" },
                }
            });
        }

        if (!string.IsNullOrWhiteSpace(config["OpenAI:ApiKey"]
            ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")))
        {
            list.Add(new AiProviderInfo
            {
                Id = "openai",
                Name = "GPT (OpenAI)",
                Models = new List<AiModelInfo>
                {
                    new() { Id = "gpt-4o-mini", Name = "GPT-4o mini", IsDefault = true },
                    new() { Id = "gpt-4o",      Name = "GPT-4o" },
                    new() { Id = "gpt-4-turbo", Name = "GPT-4 Turbo" },
                }
            });
        }

        if (!string.IsNullOrWhiteSpace(config["Gemini:ApiKey"]
            ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")))
        {
            list.Add(new AiProviderInfo
            {
                Id = "gemini",
                Name = "Gemini (Google)",
                Models = new List<AiModelInfo>
                {
                    new() { Id = "gemini-1.5-flash",     Name = "Gemini 1.5 Flash", IsDefault = true },
                    new() { Id = "gemini-1.5-pro",       Name = "Gemini 1.5 Pro" },
                    new() { Id = "gemini-2.0-flash-exp", Name = "Gemini 2.0 Flash (exp)" },
                }
            });
        }

        return new AvailableProvidersResponse { Available = list };
    }
}
