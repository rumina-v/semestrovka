using DetectiveInterrogation.Models.Entities;

namespace DetectiveInterrogation.Services.Interfaces;

public interface ICaseService
{
    Task<List<Case>> GetAllCasesAsync();
    Task<Case?> GetCaseByIdAsync(int caseId);
    Task<Case> CreateCaseAsync(string title, string? newspaperText, string? shortDescription, string? fullDescription);
    Task<bool> UpdateCaseAsync(int caseId, string title, string? newspaperText, string? shortDescription, string? fullDescription);
    Task<bool> DeleteCaseAsync(int caseId);
}
