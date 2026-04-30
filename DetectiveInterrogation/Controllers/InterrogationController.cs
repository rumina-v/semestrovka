using DetectiveInterrogation.Helpers;
using DetectiveInterrogation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> StartInterrogation([FromBody] StartInterrogationRequest request)
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
    public async Task<IActionResult> ProcessPhrase(int sessionId, [FromBody] ProcessPhraseRequest request)
    {
        var userId = _claimsHelper.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        var result = await _interrogationService.ProcessPhrasSelectionAsync(sessionId, request.PhraseId);
        if (result == null)
            return BadRequest(new { message = "Failed to process phrase" });

        return Ok(result);
    }

    [HttpGet("state/{sessionId}")]
    public async Task<IActionResult> GetSessionState(int sessionId)
    {
        var state = await _interrogationService.GetSessionStateAsync(sessionId);
        if (state == null)
            return NotFound(new { message = "Session not found" });

        return Ok(state);
    }

    [HttpPost("end/{sessionId}")]
    public async Task<IActionResult> EndInterrogation(int sessionId)
    {
        var result = await _interrogationService.EndInterrogationSessionAsync(sessionId);
        if (!result)
            return BadRequest(new { message = "Failed to end interrogation" });

        return Ok(new { message = "Interrogation ended" });
    }

    [HttpGet("phrases/{sessionId}")]
    public async Task<IActionResult> GetAvailablePhrases(int sessionId)
    {
        var phrases = await _interrogationService.GetAvailablePhrasesAsync(sessionId);
        return Ok(phrases);
    }
}

public class StartInterrogationRequest
{
    public int CaseId { get; set; }
    public int SuspectId { get; set; }
}

public class ProcessPhraseRequest
{
    public int PhraseId { get; set; }
}
