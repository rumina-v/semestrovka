using DetectiveInterrogation.Models.DTOs.Case;
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

    public async Task<List<CaseDto>> GetAllCasesAsync()
    {
        var cases = await _caseRepository.GetAllAsync();
        return cases.Select(ToDto).ToList();
    }

    public async Task<CaseDto?> GetCaseByIdAsync(int caseId)
    {
        var caseEntity = await _caseRepository.GetByIdAsync(caseId);
        return caseEntity == null ? null : ToDto(caseEntity);
    }

    public async Task<CaseDto> CreateCaseAsync(string title, string? newspaperText, string? shortDescription, string? fullDescription)
    {
        var caseEntity = new Case
        {
            Title = title,
            NewspaperText = newspaperText,
            ShortDescription = shortDescription,
            FullDescription = fullDescription
        };

        await _caseRepository.AddAsync(caseEntity);
        return ToDto(caseEntity);
    }

    public async Task<bool> UpdateCaseAsync(int caseId, string title, string? newspaperText, string? shortDescription, string? fullDescription)
    {
        return await _caseRepository.UpdateAsync(caseId, title, newspaperText, shortDescription, fullDescription);
    }

    public async Task<bool> DeleteCaseAsync(int caseId)
    {
        return await _caseRepository.DeleteAsync(caseId);
    }

    private static CaseDto ToDto(Case caseEntity)
    {
        return new CaseDto
        {
            Id = caseEntity.Id,
            Title = caseEntity.Title,
            NewspaperText = caseEntity.NewspaperText,
            ShortDescription = caseEntity.ShortDescription,
            FullDescription = caseEntity.FullDescription,
            Suspects = caseEntity.Suspects
                .OrderBy(s => s.IsGuilty)
                .ThenBy(s => s.Id)
                .Select(s => new SuspectDto
                {
                    Id = s.Id,
                    CaseId = s.CaseId,
                    Name = s.Name,
                    Description = s.Description,
                    InitialTrust = s.InitialTrust,
                    InitialPressure = s.InitialPressure,
                    IsGuilty = s.IsGuilty
                })
                .ToList(),
            Evidence = caseEntity.Evidence
                .OrderBy(e => e.Id)
                .Select(e => new EvidenceDto
                {
                    Id = e.Id,
                    CaseId = e.CaseId,
                    SuspectId = e.SuspectId,
                    Title = e.Title,
                    Description = e.Description,
                    Phrases = e.Phrases
                        .OrderBy(p => p.Id)
                        .Select(p => new EvidencePhraseDto
                        {
                            Id = p.Id,
                            EvidenceId = p.EvidenceId,
                            Text = p.Text,
                            TrustChange = p.TrustChange,
                            PressureChange = p.PressureChange
                        })
                        .ToList()
                })
                .ToList()
        };
    }
}
