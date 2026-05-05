using DetectiveInterrogation.Data;
using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DetectiveInterrogation.Repositories;

public class CaseRepository : ICaseRepository
{
    private readonly AppDbContext _context;

    public CaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Case>> GetAllAsync()
    {
        return await _context.Cases
            .Include(c => c.Suspects)
            .Include(c => c.Evidence)
            .ToListAsync();
    }

    public async Task<Case?> GetByIdAsync(int caseId)
    {
        return await _context.Cases
            .Include(c => c.Suspects)
            .Include(c => c.Evidence)
                .ThenInclude(e => e.Phrases)
            .FirstOrDefaultAsync(c => c.Id == caseId);
    }

    public async Task AddAsync(Case caseEntity)
    {
        _context.Cases.Add(caseEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(int caseId, string title, string? newspaperText, string? shortDescription, string? fullDescription)
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

    public async Task<bool> DeleteAsync(int caseId)
    {
        var caseEntity = await _context.Cases.FindAsync(caseId);
        if (caseEntity == null)
            return false;

        _context.Cases.Remove(caseEntity);
        await _context.SaveChangesAsync();
        return true;
    }
}
