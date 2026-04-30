using DetectiveInterrogation.Helpers;
using DetectiveInterrogation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DetectiveInterrogation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AchievementController : ControllerBase
{
    private readonly IAchievementService _achievementService;
    private readonly ClaimsHelper _claimsHelper;
    private readonly ILogger<AchievementController> _logger;

    public AchievementController(
        IAchievementService achievementService,
        ClaimsHelper claimsHelper,
        ILogger<AchievementController> logger)
    {
        _achievementService = achievementService;
        _claimsHelper = claimsHelper;
        _logger = logger;
    }

    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllAchievements()
    {
        var achievements = await _achievementService.GetAllAchievementsAsync();
        return Ok(achievements);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserAchievements()
    {
        var userId = _claimsHelper.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        var achievements = await _achievementService.GetUserAchievementsAsync(userId.Value);
        return Ok(achievements);
    }

    [HttpGet("{achievementId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAchievementById(int achievementId)
    {
        var achievement = await _achievementService.GetAchievementByIdAsync(achievementId);
        if (achievement == null)
            return NotFound(new { message = "Achievement not found" });

        return Ok(achievement);
    }
}
