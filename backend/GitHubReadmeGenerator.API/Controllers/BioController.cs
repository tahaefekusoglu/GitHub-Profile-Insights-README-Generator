using GitHubReadmeGenerator.API.Models;
using GitHubReadmeGenerator.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GitHubReadmeGenerator.API.Controllers;

[ApiController]
[Route("api/bio")]
public class BioController : ControllerBase
{
    private readonly ClaudeService _claude;
    private readonly OpenAiService _openai;
    private readonly GeminiService _gemini;

    public BioController(ClaudeService claude, OpenAiService openai, GeminiService gemini)
    {
        _claude = claude;
        _openai = openai;
        _gemini = gemini;
    }

    [HttpPost("generate")]
    [RequestSizeLimit(100_000)]
    [EnableRateLimiting("ip")]
    public async Task<IActionResult> Generate(
        [FromBody] GitHubProfile profile,
        [FromQuery] string? provider = null,
        [FromQuery] string? model = null)
    {
        var service = ResolveService(provider);

        if (service is null)
            return StatusCode(503, new ApiResponse<string>
            {
                Success = false,
                Error = "AI bio is not available — no API key configured (Anthropic, OpenAI, or Gemini)"
            });

        try
        {
            var bio = await service.GenerateBioAsync(profile, model);
            return Ok(new ApiResponse<string> { Success = true, Data = bio });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(503, new ApiResponse<string>
            {
                Success = false,
                Error = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Error = ex.Message
            });
        }
    }

    private IAiService? ResolveService(string? provider)
    {
        // If provider explicitly requested, use it (even if key missing — will throw 503)
        if (provider == "claude") return _claude;
        if (provider == "openai") return _openai;
        if (provider == "gemini") return _gemini;

        // Auto: pick first available
        if (_claude is { } c && IsConfigured(c)) return c;
        if (_openai is { } o && IsConfigured(o)) return o;
        if (_gemini is { } g && IsConfigured(g)) return g;
        return null;
    }

    private static bool IsConfigured(IAiService svc) => svc switch
    {
        ClaudeService cs => cs.IsConfigured,
        OpenAiService os => os.IsConfigured,
        GeminiService gs => gs.IsConfigured,
        _ => false
    };
}
