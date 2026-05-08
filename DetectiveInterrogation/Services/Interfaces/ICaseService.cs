using DetectiveInterrogation.Models.DTOs.Case;

namespace DetectiveInterrogation.Services.Interfaces;

public interface ICaseService
{
    Task<List<CaseDto>> GetAllCasesAsync();
    Task<CaseDto?> GetCaseByIdAsync(int caseId);
    Task<CaseDto> CreateCaseAsync(string title, string? newspaperText, string? shortDescription, string? fullDescription);
    Task<bool> UpdateCaseAsync(int caseId, string title, string? newspaperText, string? shortDescription, string? fullDescription);
    Task<bool> DeleteCaseAsync(int caseId);
}
