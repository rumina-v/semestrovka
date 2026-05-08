using DetectiveInterrogation.Helpers;
using DetectiveInterrogation.Models.DTOs.Auth;
using DetectiveInterrogation.Services.Interfaces;
using DetectiveInterrogation.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DetectiveInterrogation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtSettings _jwtSettings;
    private readonly JwtTokenHelper _jwtTokenHelper;
    private readonly ILogger<AuthController> _logger;

    private const string JwtCookieName = "detective.jwt";

    public AuthController(
        IAuthService authService,
        JwtSettings jwtSettings,
        JwtTokenHelper jwtTokenHelper,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _jwtSettings = jwtSettings;
        _jwtTokenHelper = jwtTokenHelper;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        SetJwtCookie(result.Token);

        _logger.LogInformation("User {Username} registered successfully", request.Username);
        return Ok(new AuthResponseDto
        {
            Message = result.Message ?? string.Empty,
            User = BuildUserResponse(result.Token)
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request.Username, request.Password);
        
        if (!result.Success)
            return Unauthorized(new { message = result.Message });

        SetJwtCookie(result.Token);

        _logger.LogInformation("User {Username} logged in successfully", request.Username);
        return Ok(new AuthResponseDto
        {
            Message = result.Message ?? string.Empty,
            User = BuildUserResponse(result.Token)
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new AuthResponseDto
        {
            Message = "Authenticated",
            User = BuildUserResponse(User)
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(JwtCookieName);
        _logger.LogInformation("User logged out");
        return Ok(new { message = "Logged out successfully" });
    }

    private void SetJwtCookie(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        Response.Cookies.Append(JwtCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes)
        });
    }

    private static AuthUserDto BuildUserResponse(ClaimsPrincipal user)
    {
        var idText = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return new AuthUserDto
        {
            Id = int.TryParse(idText, out var id) ? id : null,
            Username = user.FindFirstValue(ClaimTypes.Name),
            Email = user.FindFirstValue(ClaimTypes.Email),
            Role = user.FindFirstValue(ClaimTypes.Role)
        };
    }

    private AuthUserDto BuildUserResponse(string? token)
    {
        var user = string.IsNullOrWhiteSpace(token)
            ? new ClaimsPrincipal()
            : _jwtTokenHelper.ValidateToken(token) ?? new ClaimsPrincipal();

        return BuildUserResponse(user);
    }
}
