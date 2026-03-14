using GardenAssistant.Services;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpGet("token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDevelopmentToken()
    {
        var (accessToken, refreshToken) = await authService.GetDevelopmentTokenAsync();
        return Ok(new { accessToken, refreshToken });
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var result = await authService.RefreshAsync(request.RefreshToken);
        if (result is null) return Unauthorized();
        var (accessToken, refreshToken) = result.Value;
        return Ok(new { accessToken, refreshToken });
    }
}

public record RefreshRequest(string RefreshToken);
