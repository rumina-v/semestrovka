using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Repositories.Interfaces;
using DetectiveInterrogation.Services.Interfaces;

namespace DetectiveInterrogation.Services;

public class CaseService : ICaseService
{
    private readonly ICaseRepository _caseRepository;

    public CaseService(ICaseRepository caseRepository)
    {
        _caseRepository = caseRepository;
    }

    public async Task<List<Case>> GetAllCasesAsync()
    {
        return await _caseRepository.GetAllAsync();
    }

    public async Task<Case?> GetCaseByIdAsync(int caseId)
    {
        return await _caseRepository.GetByIdAsync(caseId);
    }

    public async Task<Case> CreateCaseAsync(string title, string? newspaperText, string? shortDescription, string? fullDescription)
    {
        var caseEntity = new Case
        {
            Title = title,
            NewspaperText = newspaperText,
            ShortDescription = shortDescription,
            FullDescription = fullDescription
        };

        await _caseRepository.AddAsync(caseEntity);
        return caseEntity;
    }

    public async Task<bool> UpdateCaseAsync(int caseId, string title, string? newspaperText, string? shortDescription, string? fullDescription)
    {
        return await _caseRepository.UpdateAsync(caseId, title, newspaperText, shortDescription, fullDescription);
    }

    public async Task<bool> DeleteCaseAsync(int caseId)
    {
        return await _caseRepository.DeleteAsync(caseId);
    }
}
