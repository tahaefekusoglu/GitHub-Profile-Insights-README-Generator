using GitHubReadmeGenerator.API.Models;
using GitHubReadmeGenerator.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GitHubReadmeGenerator.API.Controllers;

[ApiController]
[Route("api/readme")]
public class ReadmeController : ControllerBase
{
    private readonly IReadmeTemplateService _templateService;
    private static readonly HashSet<string> ValidThemes = new() { "minimal", "colorful", "dark" };

    public ReadmeController(IReadmeTemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpPost("generate")]
    [RequestSizeLimit(100_000)]
    [EnableRateLimiting("ip")]
    public IActionResult Generate([FromBody] ReadmeConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.Profile?.Login))
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Error = "Profile.Login is required."
            });

        if (!ValidThemes.Contains(config.Theme))
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Error = $"Invalid theme '{config.Theme}'. Must be one of: minimal, colorful, dark."
            });

        if (config.EnabledSections.Count == 0)
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Error = "At least one section must be enabled."
            });

        try
        {
            var markdown = _templateService.Generate(config);
            return Ok(new ApiResponse<string> { Success = true, Data = markdown });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string> { Success = false, Error = ex.Message });
        }
    }
}
