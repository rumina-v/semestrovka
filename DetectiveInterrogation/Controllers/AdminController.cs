using DetectiveInterrogation.Services.Interfaces;
using DetectiveInterrogation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DetectiveInterrogation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ClaimsHelper _claimsHelper;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IAdminService adminService, ClaimsHelper claimsHelper, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _claimsHelper = claimsHelper;
        _logger = logger;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var stats = await _adminService.GetGameStatisticsAsync();
        return Ok(stats);
    }

    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser([Range(1, int.MaxValue, ErrorMessage = "UserId must be positive")] int userId)
    {
        var result = await _adminService.DeleteUserAsync(userId);
        if (!result)
            return NotFound(new { message = "User not found" });

        _logger.LogInformation("User {UserId} deleted by admin", userId);
        return Ok(new { message = "User deleted successfully" });
    }

    [HttpPost("test-email")]
    public async Task<IActionResult> SendTestEmail()
    {
        var userId = _claimsHelper.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        var sent = await _adminService.SendTestEmailAsync(userId.Value);
        if (!sent)
            return BadRequest(new { message = "Failed to send test email. Check SMTP settings and logs." });

        _logger.LogInformation("SMTP test email sent by admin {UserId}", userId.Value);
        return Ok(new { message = "Test email sent to the current admin email." });
    }
}
