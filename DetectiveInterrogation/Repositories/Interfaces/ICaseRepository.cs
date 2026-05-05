using DetectiveInterrogation.Models.Entities;

namespace DetectiveInterrogation.Repositories.Interfaces;

public interface ICaseRepository
{
    Task<List<Case>> GetAllAsync();
    Task<Case?> GetByIdAsync(int caseId);
    Task AddAsync(Case caseEntity);
    Task<bool> UpdateAsync(int caseId, string title, string? newspaperText, string? shortDescription, string? fullDescription);
    Task<bool> DeleteAsync(int caseId);
}
