using DetectiveInterrogation.Helpers;
using DetectiveInterrogation.Models.DTOs.Interrogation;
using DetectiveInterrogation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DetectiveInterrogation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterrogationController : ControllerBase
{
    private readonly IInterrogationService _interrogationService;
    private readonly ClaimsHelper _claimsHelper;
    private readonly ILogger<InterrogationController> _logger;

    public InterrogationController(
        IInterrogationService interrogationService,
        ClaimsHelper claimsHelper,
        ILogger<InterrogationController> logger)
    {
        _interrogationService = interrogationService;
        _claimsHelper = claimsHelper;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartInterrogation([FromBody] StartInterrogationDto request)
    {
        var userId = _claimsHelper.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        var result = await _interrogationService.StartInterrogationSessionAsync(
            userId.Value, request.CaseId, request.SuspectId);

        if (result == null)
            return BadRequest(new { message = "Failed to start interrogation" });

        _logger.LogInformation("Interrogation started for user {UserId}", userId);
        return Ok(result);
    }

    [HttpPost("phrase/{sessionId}")]
    public async Task<IActionResult> ProcessPhrase(
        [Range(1, int.MaxValue, ErrorMessage = "SessionId must be positive")] int sessionId,
        [FromBody] ProcessPhraseDto request)
    {
        var userId = _claimsHelper.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        var result = await _interrogationService.ProcessPhrasSelectionAsync(userId.Value, sessionId, request.PhraseId);
        if (result == null)
            return BadRequest(new { message = "Failed to process phrase" });

        return Ok(result);
    }

    [HttpGet("state/{sessionId}")]
    public async Task<IActionResult> GetSessionState([Range(1, int.MaxValue, ErrorMessage = "SessionId must be positive")] int sessionId)
    {
        var userId = _claimsHelper.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        var state = await _interrogationService.GetSessionStateAsync(userId.Value, sessionId);
        if (state == null)
            return NotFound(new { message = "Session not found" });

        return Ok(state);
    }

    [HttpGet("ending/{caseId}")]
    public async Task<IActionResult> GetEnding([Range(1, int.MaxValue, ErrorMessage = "CaseId must be positive")] int caseId)
    {
        var userId = _claimsHelper.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        var ending = await _interrogationService.GetCaseEndingAsync(userId.Value, caseId);
        if (ending == null)
            return NotFound(new { message = "Completed interrogations not found" });

        return Ok(ending);
    }

    [HttpPost("end/{sessionId}")]
    public async Task<IActionResult> EndInterrogation([Range(1, int.MaxValue, ErrorMessage = "SessionId must be positive")] int sessionId)
    {
        var userId = _claimsHelper.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        var result = await _interrogationService.EndInterrogationSessionAsync(userId.Value, sessionId);
        if (!result)
            return BadRequest(new { message = "Failed to end interrogation" });

        return Ok(new { message = "Interrogation ended" });
    }

    [HttpGet("phrases/{sessionId}")]
    public async Task<IActionResult> GetAvailablePhrases([Range(1, int.MaxValue, ErrorMessage = "SessionId must be positive")] int sessionId)
    {
        var userId = _claimsHelper.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        var phrases = await _interrogationService.GetAvailablePhrasesAsync(userId.Value, sessionId);
        return Ok(phrases);
    }
}
