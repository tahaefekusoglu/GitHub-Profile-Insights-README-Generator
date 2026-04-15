using GitHubReadmeGenerator.API.Models;
using GitHubReadmeGenerator.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GitHubReadmeGenerator.API.Controllers;

[ApiController]
[Route("api/bio")]
public class BioController : ControllerBase
{
    private readonly IAiService _aiService;

    public BioController(IAiService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("generate")]
    [RequestSizeLimit(100_000)]
    [EnableRateLimiting("ip")]
    public async Task<IActionResult> Generate([FromBody] GitHubProfile profile)
    {
        try
        {
            var bio = await _aiService.GenerateBioAsync(profile);
            return Ok(new ApiResponse<string> { Success = true, Data = bio });
        }
        catch (InvalidOperationException)
        {
            return StatusCode(503, new ApiResponse<string>
            {
                Success = false,
                Error = "AI bio is not available — no API key configured (Anthropic, OpenAI, or Gemini)"
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
}
