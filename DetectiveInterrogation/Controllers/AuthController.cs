using DetectiveInterrogation.Helpers;
using DetectiveInterrogation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DetectiveInterrogation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        _logger.LogInformation("User {Username} registered successfully", request.Username);
        return Ok(new { token = result.Token, message = result.Message });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request.Username, request.Password);
        
        if (!result.Success)
            return Unauthorized(new { message = result.Message });

        _logger.LogInformation("User {Username} logged in successfully", request.Username);
        return Ok(new { token = result.Token, message = result.Message });
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        _logger.LogInformation("User logged out");
        return Ok(new { message = "Logged out successfully" });
    }
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
