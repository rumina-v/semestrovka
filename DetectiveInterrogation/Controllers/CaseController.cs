using DetectiveInterrogation.Helpers;
using DetectiveInterrogation.Models.DTOs.Case;
using DetectiveInterrogation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
    public async Task<IActionResult> GetCaseById([Range(1, int.MaxValue, ErrorMessage = "CaseId must be positive")] int caseId)
    {
        var caseEntity = await _caseService.GetCaseByIdAsync(caseId);
        if (caseEntity == null)
            return NotFound(new { message = "Case not found" });

        return Ok(caseEntity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCase([FromBody] CreateCaseDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var caseEntity = await _caseService.CreateCaseAsync(
            request.Title, request.NewspaperText, request.ShortDescription, request.FullDescription);

        _logger.LogInformation("Case {CaseTitle} created", caseEntity.Title);
        return CreatedAtAction(nameof(GetCaseById), new { caseId = caseEntity.Id }, caseEntity);
    }
}
