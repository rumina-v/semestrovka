using DetectiveInterrogation.Data;
using DetectiveInterrogation.Models.DTOs.Interrogation;
using DetectiveInterrogation.Models.Entities;
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

    public async Task<SuspectReply?> GetReplyAsync(int suspectId, int phraseId, int trust, int pressure)
    {
        return await _context.SuspectReplies
            .Where(sr => sr.SuspectId == suspectId
                && sr.EvidencePhraseId == phraseId
                && sr.MinTrust <= trust
                && sr.MaxTrust >= trust
                && sr.MinPressure <= pressure
                && sr.MaxPressure >= pressure)
            .OrderBy(sr => sr.Id)
            .FirstOrDefaultAsync();
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

    public async Task<List<AvailablePhraseDto>> GetAvailablePhrasesAsync(int suspectId, List<int> usedEvidenceIds)
    {
        var suspectIdText = suspectId.ToString();

        return await _context.EvidencePhrases
            .Where(ep => !usedEvidenceIds.Contains(ep.EvidenceId)
                && (ep.Evidence.SuspectId == suspectIdText
                    || ep.Evidence.SuspectId.StartsWith(suspectIdText + ",")
                    || ep.Evidence.SuspectId.EndsWith("," + suspectIdText)
                    || ep.Evidence.SuspectId.Contains("," + suspectIdText + ",")))
            .Include(ep => ep.Evidence)
            .Select(ep => new AvailablePhraseDto
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
