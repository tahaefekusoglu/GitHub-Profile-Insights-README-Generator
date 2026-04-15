using System.Text.RegularExpressions;
using GitHubReadmeGenerator.API.Models;
using GitHubReadmeGenerator.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GitHubReadmeGenerator.API.Controllers;

[ApiController]
[Route("api/github")]
public class GitHubController : ControllerBase
{
    private readonly IGitHubService _githubService;

    public GitHubController(IGitHubService githubService)
    {
        _githubService = githubService;
    }

    private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9\-]{1,39}$", RegexOptions.Compiled);

    [HttpGet("{username}")]
    public async Task<IActionResult> GetProfile(string username)
    {
        if (!UsernameRegex.IsMatch(username))
            return BadRequest(new ApiResponse<GitHubProfile>
            {
                Success = false,
                Error = "Invalid GitHub username format."
            });

        try
        {
            var profile = await _githubService.GetProfileAsync(username);
            if (profile is null)
                return NotFound(new ApiResponse<GitHubProfile>
                {
                    Success = false,
                    Error = $"GitHub user '{username}' not found."
                });

            return Ok(new ApiResponse<GitHubProfile> { Success = true, Data = profile });
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(429, new ApiResponse<GitHubProfile>
            {
                Success = false,
                Error = "GitHub rate limit exceeded. Try again in 60 seconds."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<GitHubProfile>
            {
                Success = false,
                Error = ex.Message
            });
        }
    }
}
