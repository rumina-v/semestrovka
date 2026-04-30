using DetectiveInterrogation.Data;
using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DetectiveInterrogation.Services;

public class InterrogationService : IInterrogationService
{
    private readonly AppDbContext _context;
    private readonly IAchievementService _achievementService;
    private readonly ILogger<InterrogationService> _logger;

    public InterrogationService(AppDbContext context, IAchievementService achievementService, ILogger<InterrogationService> logger)
    {
        _context = context;
        _achievementService = achievementService;
        _logger = logger;
    }

    /// <summary>
    /// Starts a new interrogation session
    /// </summary>
    public async Task<object?> StartInterrogationSessionAsync(int userId, int caseId, int suspectId)
    {
        try
        {
            var suspect = await _context.Suspects.FindAsync(suspectId);
            if (suspect == null)
                return null;

            var session = new InterrogationSession
            {
                UserId = userId,
                CaseId = caseId,
                SuspectId = suspectId,
                CurrentTrust = suspect.InitialTrust,
                CurrentAggression = suspect.InitialAggression,
                Status = "InProgress"
            };

            _context.InterrogationSessions.Add(session);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Interrogation session started for user {UserId}, suspect {SuspectId}", userId, suspectId);

            return new
            {
                session.Id,
                session.CurrentTrust,
                session.CurrentAggression,
                SuspectName = suspect.Name,
                Message = "Interrogation session started. Choose a phrase to begin."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting interrogation session");
            return null;
        }
    }

    /// <summary>
    /// Processes the selection of a phrase by the detective
    /// </summary>
    public async Task<object?> ProcessPhrasSelectionAsync(int sessionId, int phraseId)
    {
        try
        {
            var session = await _context.InterrogationSessions.FindAsync(sessionId);
            if (session == null)
                return null;

            var phrase = await _context.EvidencePhrases.FindAsync(phraseId);
            if (phrase == null)
                return null;

            var evidence = await _context.Evidence.FindAsync(phrase.EvidenceId);
            if (evidence == null)
                return null;

            // Check if evidence already used in this session
            var alreadyUsed = await _context.SessionUsedEvidences
                .AnyAsync(sue => sue.SessionId == sessionId && sue.EvidenceId == evidence.Id);

            if (alreadyUsed)
                return new { Error = "This evidence has already been used in this interrogation session" };

            // Mark evidence as used
            var sessionUsedEvidence = new SessionUsedEvidence
            {
                SessionId = sessionId,
                EvidenceId = evidence.Id
            };
            _context.SessionUsedEvidences.Add(sessionUsedEvidence);

            // Get the suspect's reply for this phrase
            var reply = await _context.SuspectReplies
                .FirstOrDefaultAsync(sr => sr.SuspectId == session.SuspectId && sr.PhraseId == phraseId);

            if (reply == null)
                return new { Error = "No reply found for this phrase" };

            // Update session parameters
            session.CurrentTrust = Math.Clamp(session.CurrentTrust + reply.TrustChange, 0, 100);
            session.CurrentAggression = Math.Clamp(session.CurrentAggression + reply.AggressionChange, 0, 100);

            // Check if interrogation should end
            CheckInterrogationEnd(session);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Phrase processed in session {SessionId}: Trust={Trust}, Aggression={Aggression}",
                sessionId, session.CurrentTrust, session.CurrentAggression);

            return new
            {
                ReplyText = reply.ReplyText,
                session.CurrentTrust,
                session.CurrentAggression,
                session.Status,
                EvidenceTitle = evidence.Title
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing phrase selection");
            return null;
        }
    }

    /// <summary>
    /// Gets the current state of an interrogation session
    /// </summary>
    public async Task<object?> GetSessionStateAsync(int sessionId)
    {
        try
        {
            var session = await _context.InterrogationSessions
                .Include(s => s.Suspect)
                .Include(s => s.Case)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                return null;

            var usedEvidenceIds = await _context.SessionUsedEvidences
                .Where(sue => sue.SessionId == sessionId)
                .Select(sue => sue.EvidenceId)
                .ToListAsync();

            return new
            {
                session.Id,
                session.CurrentTrust,
                session.CurrentAggression,
                session.Status,
                SuspectName = session.Suspect.Name,
                CaseName = session.Case.Title,
                UsedEvidenceCount = usedEvidenceIds.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting session state");
            return null;
        }
    }

    /// <summary>
    /// Ends the interrogation session and calculates results
    /// </summary>
    public async Task<bool> EndInterrogationSessionAsync(int sessionId)
    {
        try
        {
            var session = await _context.InterrogationSessions.FindAsync(sessionId);
            if (session == null)
                return false;

            session.Status = "Completed";
            await _context.SaveChangesAsync();

            // Award achievement for completing interrogation
            var firstInterrogationAchievement = await _context.Achievements
                .FirstOrDefaultAsync(a => a.Title == "First Interrogation");

            if (firstInterrogationAchievement != null)
            {
                await _achievementService.AwardAchievementAsync(session.UserId, firstInterrogationAchievement.Id);
            }

            _logger.LogInformation("Interrogation session {SessionId} completed", sessionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending interrogation session");
            return false;
        }
    }

    /// <summary>
    /// Gets available phrases for the current session state
    /// </summary>
    public async Task<List<object>> GetAvailablePhrasesAsync(int sessionId)
    {
        try
        {
            var session = await _context.InterrogationSessions.FindAsync(sessionId);
            if (session == null)
                return new List<object>();

            var usedEvidenceIds = await _context.SessionUsedEvidences
                .Where(sue => sue.SessionId == sessionId)
                .Select(sue => sue.EvidenceId)
                .ToListAsync();

            var availablePhrases = await _context.EvidencePhrases
                .Where(ep => !usedEvidenceIds.Contains(ep.EvidenceId))
                .Include(ep => ep.Evidence)
                .Select(ep => new
                {
                    ep.Id,
                    ep.Text,
                    EvidenceTitle = ep.Evidence.Title
                })
                .ToListAsync();

            return availablePhrases.Cast<object>().ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available phrases");
            return new List<object>();
        }
    }

    /// <summary>
    /// Awards an achievement to a user
    /// </summary>
    public async Task<bool> AwardAchievementAsync(int userId, int achievementId)
    {
        return await _achievementService.AwardAchievementAsync(userId, achievementId);
    }

    /// <summary>
    /// Checks if interrogation should end based on current parameters
    /// </summary>
    private void CheckInterrogationEnd(InterrogationSession session)
    {
        // End if trust is maxed and aggression is maxed (confession)
        if (session.CurrentTrust == 100 && session.CurrentAggression == 100)
        {
            session.Status = "Confession";
            _logger.LogInformation("Suspect confessed in session {SessionId}", session.Id);
        }
        // End if trust is zero and aggression is maxed (suspect refuses to talk)
        else if (session.CurrentTrust == 0 && session.CurrentAggression == 100)
        {
            session.Status = "Refused";
            _logger.LogInformation("Suspect refused to talk in session {SessionId}", session.Id);
        }
    }
}
