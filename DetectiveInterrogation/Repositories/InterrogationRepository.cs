using DetectiveInterrogation.Data;
using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Models.ViewModels.Interrogation;
using DetectiveInterrogation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DetectiveInterrogation.Repositories;

public class InterrogationRepository : IInterrogationRepository
{
    private readonly AppDbContext _context;

    public InterrogationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Suspect?> GetSuspectByIdAsync(int suspectId)
    {
        return await _context.Suspects.FindAsync(suspectId);
    }

    public async Task<InterrogationSession?> GetSessionByIdAsync(int sessionId)
    {
        return await _context.InterrogationSessions.FindAsync(sessionId);
    }

    public async Task<InterrogationSession?> GetSessionWithCaseAndSuspectAsync(int sessionId)
    {
        return await _context.InterrogationSessions
            .Include(s => s.Suspect)
            .Include(s => s.Case)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    public async Task<InterrogationSession?> GetSessionWithResultDetailsAsync(int sessionId)
    {
        return await _context.InterrogationSessions
            .Include(s => s.User)
            .Include(s => s.Case)
            .Include(s => s.Suspect)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    public async Task<EvidencePhrase?> GetPhraseByIdAsync(int phraseId)
    {
        return await _context.EvidencePhrases.FindAsync(phraseId);
    }

    public async Task<Evidence?> GetEvidenceByIdAsync(int evidenceId)
    {
        return await _context.Evidence.FindAsync(evidenceId);
    }

    public async Task<SuspectReply?> GetReplyAsync(int suspectId, int phraseId)
    {
        return await _context.SuspectReplies
            .FirstOrDefaultAsync(sr => sr.SuspectId == suspectId && sr.PhraseId == phraseId);
    }

    public async Task<bool> IsEvidenceUsedAsync(int sessionId, int evidenceId)
    {
        return await _context.SessionUsedEvidences
            .AnyAsync(sue => sue.SessionId == sessionId && sue.EvidenceId == evidenceId);
    }

    public async Task<List<int>> GetUsedEvidenceIdsAsync(int sessionId)
    {
        return await _context.SessionUsedEvidences
            .Where(sue => sue.SessionId == sessionId)
            .Select(sue => sue.EvidenceId)
            .ToListAsync();
    }

    public async Task<List<AvailablePhraseViewModel>> GetAvailablePhrasesAsync(int suspectId, List<int> usedEvidenceIds)
    {
        return await _context.EvidencePhrases
            .Where(ep => !usedEvidenceIds.Contains(ep.EvidenceId)
                && ep.SuspectReplies.Any(sr => sr.SuspectId == suspectId))
            .Include(ep => ep.Evidence)
            .Select(ep => new AvailablePhraseViewModel
            {
                Id = ep.Id,
                Text = ep.Text,
                EvidenceId = ep.EvidenceId,
                EvidenceTitle = ep.Evidence.Title
            })
            .ToListAsync();
    }

    public async Task<List<InterrogationSession>> GetLatestCaseSessionsAsync(int userId, int caseId, int count)
    {
        return await _context.InterrogationSessions
            .Where(s => s.UserId == userId && s.CaseId == caseId && s.Status != "InProgress")
            .Include(s => s.Case)
            .Include(s => s.Suspect)
            .OrderByDescending(s => s.Id)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Achievement?> GetAchievementByTitleAsync(string title)
    {
        return await _context.Achievements.FirstOrDefaultAsync(a => a.Title == title);
    }

    public async Task<List<string>> GetAchievementTitlesByUserIdAsync(int userId)
    {
        return await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Achievement)
            .Select(ua => ua.Achievement.Title)
            .ToListAsync();
    }

    public async Task AddSessionAsync(InterrogationSession session)
    {
        _context.InterrogationSessions.Add(session);
        await _context.SaveChangesAsync();
    }

    public void AddSessionUsedEvidence(SessionUsedEvidence sessionUsedEvidence)
    {
        _context.SessionUsedEvidences.Add(sessionUsedEvidence);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
