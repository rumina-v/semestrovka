using DetectiveInterrogation.Helpers;
using DetectiveInterrogation.Models.Entities;
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
        return Ok(cases.Select(ToCaseSummary));
    }

    [HttpGet("main-newspaper")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMainNewspaper()
    {
        var caseEntity = (await _caseService.GetAllCasesAsync()).FirstOrDefault();
        if (caseEntity == null)
            return NotFound(new { message = "Case not found" });

        return Ok(new
        {
            caseEntity.Id,
            caseEntity.Title,
            caseEntity.NewspaperText
        });
    }

    [HttpGet("{caseId}")]
    public async Task<IActionResult> GetCaseById(int caseId)
    {
        var caseEntity = await _caseService.GetCaseByIdAsync(caseId);
        if (caseEntity == null)
            return NotFound(new { message = "Case not found" });

        return Ok(ToCaseDetails(caseEntity));
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
        return CreatedAtAction(nameof(GetCaseById), new { caseId = caseEntity.Id }, ToCaseSummary(caseEntity));
    }

    private static object ToCaseSummary(Case caseEntity)
    {
        return new
        {
            caseEntity.Id,
            caseEntity.Title,
            caseEntity.NewspaperText,
            caseEntity.ShortDescription,
            caseEntity.FullDescription,
            Suspects = caseEntity.Suspects.Select(s => new
            {
                s.Id,
                s.CaseId,
                s.Name,
                s.Description,
                s.InitialTrust,
                s.InitialAggression,
                s.IsGuilty
            }),
            Evidence = caseEntity.Evidence.Select(e => new
            {
                e.Id,
                e.CaseId,
                e.Title,
                e.ShortText,
                e.FullText
            })
        };
    }

    private static object ToCaseDetails(Case caseEntity)
    {
        return new
        {
            caseEntity.Id,
            caseEntity.Title,
            caseEntity.NewspaperText,
            caseEntity.ShortDescription,
            caseEntity.FullDescription,
            Suspects = caseEntity.Suspects.Select(s => new
            {
                s.Id,
                s.CaseId,
                s.Name,
                s.Description,
                s.InitialTrust,
                s.InitialAggression,
                s.IsGuilty
            }),
            Evidence = caseEntity.Evidence.Select(e => new
            {
                e.Id,
                e.CaseId,
                e.Title,
                e.ShortText,
                e.FullText,
                Phrases = e.Phrases.Select(p => new
                {
                    p.Id,
                    p.EvidenceId,
                    p.Text
                })
            })
        };
    }
}

public class CreateCaseRequest
{
    public string Title { get; set; } = string.Empty;
    public string? NewspaperText { get; set; }
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
}
