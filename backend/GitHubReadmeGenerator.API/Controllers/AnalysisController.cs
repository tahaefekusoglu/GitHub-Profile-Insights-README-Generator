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
    private readonly IAiService _aiService;

    public AnalysisController(IGitHubService github, IAiService aiService)
    {
        _github = github;
        _aiService = aiService;
    }

    [HttpGet("api/analysis/{username}")]
    public async Task<IActionResult> GetAnalysis(string username)
    {
        if (!UsernameRegex.IsMatch(username))
        {
            return BadRequest(new ApiResponse<ProfileAnalysis>
            {
                Success = false,
                Error = "Invalid GitHub username format."
            });
        }

        var profile = await _github.GetProfileAsync(username);
        if (profile is null)
        {
            return NotFound(new ApiResponse<ProfileAnalysis>
            {
                Success = false,
                Error = $"GitHub user '{username}' not found."
            });
        }

        try
        {
            var analysis = await _aiService.GenerateAnalysisAsync(profile);
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
}
