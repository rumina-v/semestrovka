using DetectiveInterrogation.Data;
using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DetectiveInterrogation.Services;

public class CaseService : ICaseService
{
    private readonly AppDbContext _context;

    public CaseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Case>> GetAllCasesAsync()
    {
        return await _context.Cases
            .Include(c => c.Suspects)
            .Include(c => c.Evidence)
            .ToListAsync();
    }

    public async Task<Case?> GetCaseByIdAsync(int caseId)
    {
        return await _context.Cases
            .Include(c => c.Suspects)
            .Include(c => c.Evidence)
                .ThenInclude(e => e.Phrases)
            .FirstOrDefaultAsync(c => c.Id == caseId);
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

        _context.Cases.Add(caseEntity);
        await _context.SaveChangesAsync();
        return caseEntity;
    }

    public async Task<bool> UpdateCaseAsync(int caseId, string title, string? newspaperText, string? shortDescription, string? fullDescription)
    {
        var caseEntity = await _context.Cases.FindAsync(caseId);
        if (caseEntity == null)
            return false;

        caseEntity.Title = title;
        caseEntity.NewspaperText = newspaperText;
        caseEntity.ShortDescription = shortDescription;
        caseEntity.FullDescription = fullDescription;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCaseAsync(int caseId)
    {
        var caseEntity = await _context.Cases.FindAsync(caseId);
        if (caseEntity == null)
            return false;

        _context.Cases.Remove(caseEntity);
        await _context.SaveChangesAsync();
        return true;
    }
}
