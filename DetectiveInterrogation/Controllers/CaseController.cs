using DetectiveInterrogation.Helpers;
using DetectiveInterrogation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DetectiveInterrogation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CaseController : ControllerBase
{
    private readonly ICaseService _caseService;
    private readonly ClaimsHelper _claimsHelper;
    private readonly ILogger<CaseController> _logger;

    public CaseController(ICaseService caseService, ClaimsHelper claimsHelper, ILogger<CaseController> logger)
    {
        _caseService = caseService;
        _claimsHelper = claimsHelper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCases()
    {
        var cases = await _caseService.GetAllCasesAsync();
        return Ok(cases);
    }

    [HttpGet("{caseId}")]
    public async Task<IActionResult> GetCaseById(int caseId)
    {
        var caseEntity = await _caseService.GetCaseByIdAsync(caseId);
        if (caseEntity == null)
            return NotFound(new { message = "Case not found" });

        return Ok(caseEntity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCase([FromBody] CreateCaseRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var caseEntity = await _caseService.CreateCaseAsync(
            request.Title, request.NewspaperText, request.ShortDescription, request.FullDescription);

        _logger.LogInformation("Case {CaseTitle} created", caseEntity.Title);
        return CreatedAtAction(nameof(GetCaseById), new { caseId = caseEntity.Id }, caseEntity);
    }
}

public class CreateCaseRequest
{
    public string Title { get; set; } = string.Empty;
    public string? NewspaperText { get; set; }
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
}
