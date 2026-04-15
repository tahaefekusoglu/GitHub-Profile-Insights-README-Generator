using GitHubReadmeGenerator.API.Models;
using GitHubReadmeGenerator.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GitHubReadmeGenerator.API.Controllers;

[ApiController]
public class AiProvidersController : ControllerBase
{
    private readonly IConfiguration _config;

    public AiProvidersController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet("api/ai/providers")]
    public IActionResult GetProviders()
    {
        var result = AiProviderFactory.GetAvailableProviders(_config);
        return Ok(new ApiResponse<AvailableProvidersResponse> { Success = true, Data = result });
    }
}
