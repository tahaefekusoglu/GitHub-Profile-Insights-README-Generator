using System.Text.RegularExpressions;
using GitHubReadmeGenerator.API.Models;
using GitHubReadmeGenerator.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GitHubReadmeGenerator.API.Controllers;

[ApiController]
[EnableRateLimiting("ip")]
public class AnalysisController : ControllerBase
{
    private static readonly Regex UsernameRegex =
        new(@"^[a-zA-Z0-9\-]{1,39}$", RegexOptions.Compiled);

    private readonly IGitHubService _github;
    private readonly ClaudeService _claude;
    private readonly OpenAiService _openai;
    private readonly GeminiService _gemini;

    public AnalysisController(
        IGitHubService github,
        ClaudeService claude,
        OpenAiService openai,
        GeminiService gemini)
    {
        _github = github;
        _claude = claude;
        _openai = openai;
        _gemini = gemini;
    }

    [HttpGet("api/analysis/{username}")]
    public async Task<IActionResult> GetAnalysis(
        string username,
        [FromQuery] string? provider = null,
        [FromQuery] string? model = null)
    {
        if (!UsernameRegex.IsMatch(username))
            return BadRequest(new ApiResponse<ProfileAnalysis>
            {
                Success = false,
                Error = "Invalid GitHub username format."
            });

        var profile = await _github.GetProfileAsync(username);
        if (profile is null)
            return NotFound(new ApiResponse<ProfileAnalysis>
            {
                Success = false,
                Error = $"GitHub user '{username}' not found."
            });

        var service = ResolveService(provider);

        try
        {
            ProfileAnalysis analysis;
            if (service is not null)
                analysis = await service.GenerateAnalysisAsync(profile, model);
            else
                analysis = LocalProfileAnalyzer.Analyze(profile);

            return Ok(new ApiResponse<ProfileAnalysis> { Success = true, Data = analysis });
        }
        catch (InvalidOperationException)
        {
            var analysis = LocalProfileAnalyzer.Analyze(profile);
            return Ok(new ApiResponse<ProfileAnalysis> { Success = true, Data = analysis });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ProfileAnalysis>
            {
                Success = false,
                Error = ex.Message
            });
        }
    }

    private IAiService? ResolveService(string? provider) => provider switch
    {
        "claude" => _claude,
        "openai" => _openai,
        "gemini" => _gemini,
        _ => null
    };
}
