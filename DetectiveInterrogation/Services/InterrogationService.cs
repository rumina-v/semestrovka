using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Repositories.Interfaces;
using DetectiveInterrogation.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace DetectiveInterrogation.Services;

public class InterrogationService : IInterrogationService
{
    private readonly IInterrogationRepository _interrogationRepository;
    private readonly IAchievementService _achievementService;
    private readonly IEmailService _emailService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<InterrogationService> _logger;

    public InterrogationService(
        IInterrogationRepository interrogationRepository,
        IAchievementService achievementService,
        IEmailService emailService,
        IWebHostEnvironment environment,
        ILogger<InterrogationService> logger)
    {
        _interrogationRepository = interrogationRepository;
        _achievementService = achievementService;
        _emailService = emailService;
        _environment = environment;
        _logger = logger;
    }

    public async Task<object?> StartInterrogationSessionAsync(int userId, int caseId, int suspectId)
    {
        try
        {
            var suspect = await _interrogationRepository.GetSuspectByIdAsync(suspectId);
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

            await _interrogationRepository.AddSessionAsync(session);

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

    public async Task<object?> ProcessPhrasSelectionAsync(int sessionId, int phraseId)
    {
        try
        {
            var session = await _interrogationRepository.GetSessionByIdAsync(sessionId);
            if (session == null)
                return null;

            var phrase = await _interrogationRepository.GetPhraseByIdAsync(phraseId);
            if (phrase == null)
                return null;

            var evidence = await _interrogationRepository.GetEvidenceByIdAsync(phrase.EvidenceId);
            if (evidence == null)
                return null;

            var alreadyUsed = await _interrogationRepository.IsEvidenceUsedAsync(sessionId, evidence.Id);

            if (alreadyUsed)
                return new { Error = "This evidence has already been used in this interrogation session" };

            var sessionUsedEvidence = new SessionUsedEvidence
            {
                SessionId = sessionId,
                EvidenceId = evidence.Id
            };
            _interrogationRepository.AddSessionUsedEvidence(sessionUsedEvidence);

            var reply = await _interrogationRepository.GetReplyAsync(session.SuspectId, phraseId);

            if (reply == null)
                return new { Error = "No reply found for this phrase" };

            session.CurrentTrust = Math.Clamp(session.CurrentTrust + reply.TrustChange, 0, 100);
            session.CurrentAggression = Math.Clamp(session.CurrentAggression + reply.AggressionChange, 0, 100);

            CheckInterrogationEnd(session);

            await _interrogationRepository.SaveChangesAsync();

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

    public async Task<object?> GetSessionStateAsync(int sessionId)
    {
        try
        {
            var session = await _interrogationRepository.GetSessionWithCaseAndSuspectAsync(sessionId);

            if (session == null)
                return null;

            var usedEvidenceIds = await _interrogationRepository.GetUsedEvidenceIdsAsync(sessionId);

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

    public async Task<bool> EndInterrogationSessionAsync(int sessionId)
    {
        try
        {
            var session = await _interrogationRepository.GetSessionWithResultDetailsAsync(sessionId);
            if (session == null)
                return false;

            if (session.Status == "InProgress")
            {
                session.Status = "Completed";
            }

            await _interrogationRepository.SaveChangesAsync();

            var firstInterrogationAchievement = await _interrogationRepository.GetAchievementByTitleAsync("First Interrogation");

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

    public async Task<List<object>> GetAvailablePhrasesAsync(int sessionId)
    {
        try
        {
            var session = await _interrogationRepository.GetSessionByIdAsync(sessionId);
            if (session == null)
                return new List<object>();

            var usedEvidenceIds = await _interrogationRepository.GetUsedEvidenceIdsAsync(sessionId);

            var availablePhrases = await _interrogationRepository.GetAvailablePhrasesAsync(session.SuspectId, usedEvidenceIds);

            return availablePhrases.Cast<object>().ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available phrases");
            return new List<object>();
        }
    }

    public async Task<bool> AwardAchievementAsync(int userId, int achievementId)
    {
        return await _achievementService.AwardAchievementAsync(userId, achievementId);
    }

    public async Task<object?> GetCaseEndingAsync(int userId, int caseId)
    {
        try
        {
            var sessions = await _interrogationRepository.GetLatestCaseSessionsAsync(userId, caseId, 2);
            if (sessions.Count == 0)
                return null;

            sessions = sessions.OrderBy(s => s.Id).ToList();
            var caseEntity = sessions.Last().Case;
            var achievements = await _interrogationRepository.GetAchievementTitlesByUserIdAsync(userId);
            var story = await GetFullCaseStoryAsync(caseEntity.FullDescription);

            var hasConfession = sessions.Any(s => s.Status == "Confession");
            var hasRefusal = sessions.Any(s => s.Status == "Refused");
            var averageTrust = (int)Math.Round(sessions.Average(s => s.CurrentTrust));
            var averageAggression = (int)Math.Round(sessions.Average(s => s.CurrentAggression));

            var resultTitle = hasConfession
                ? "Признание получено"
                : hasRefusal
                    ? "Допрос сорвался"
                    : "Расследование завершено";

            var resultText = BuildEndingText(sessions, story, hasConfession, hasRefusal);

            return new
            {
                CaseId = caseId,
                CaseTitle = caseEntity.Title,
                ResultTitle = resultTitle,
                ResultText = resultText,
                AverageTrust = averageTrust,
                AverageAggression = averageAggression,
                Sessions = sessions.Select(s => new
                {
                    s.Id,
                    SuspectName = s.Suspect.Name,
                    s.Status,
                    s.CurrentTrust,
                    s.CurrentAggression
                }),
                Achievements = achievements
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building ending for user {UserId}, case {CaseId}", userId, caseId);
            return null;
        }
    }

    private void CheckInterrogationEnd(InterrogationSession session)
    {
        if (session.CurrentTrust == 100 && session.CurrentAggression == 100)
        {
            session.Status = "Confession";
            _logger.LogInformation("Suspect confessed in session {SessionId}", session.Id);
        }
        else if (session.CurrentTrust == 0 && session.CurrentAggression == 100)
        {
            session.Status = "Refused";
            _logger.LogInformation("Suspect refused to talk in session {SessionId}", session.Id);
        }
    }

    private async Task<string> GetFullCaseStoryAsync(string? fallbackStory)
    {
        var storyPath = Path.Combine(_environment.ContentRootPath, "story.md");
        if (!File.Exists(storyPath))
        {
            _logger.LogWarning("Story file not found at {StoryPath}", storyPath);
            return fallbackStory ?? "Full case story is not available.";
        }

        var markdown = await File.ReadAllTextAsync(storyPath, Encoding.UTF8);
        var fullStory = ExtractFullCaseStory(markdown);
        return string.IsNullOrWhiteSpace(fullStory)
            ? fallbackStory ?? "Full case story is not available."
            : fullStory;
    }

    private static string ExtractFullCaseStory(string markdown)
    {
        const string heading = "Полная история дела";
        var start = markdown.IndexOf(heading, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
            return string.Empty;

        return markdown[start..].Trim();
    }

    private static string BuildStoryAttachment(
        string username,
        string caseTitle,
        string suspectName,
        string finalStatus,
        string fullStory,
        IReadOnlyCollection<string> achievements)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Detective Interrogation");
        builder.AppendLine("Case completion report");
        builder.AppendLine();
        builder.AppendLine($"Detective: {username}");
        builder.AppendLine($"Case: {caseTitle}");
        builder.AppendLine($"Suspect: {suspectName}");
        builder.AppendLine($"Final status: {finalStatus}");
        builder.AppendLine();
        builder.AppendLine("Achievements");
        if (achievements.Count == 0)
        {
            builder.AppendLine("- No achievements earned yet");
        }
        else
        {
            foreach (var achievement in achievements)
            {
                builder.AppendLine($"- {achievement}");
            }
        }
        builder.AppendLine();
        builder.AppendLine(fullStory);
        return builder.ToString();
    }

    private static string BuildEndingText(
        IReadOnlyCollection<InterrogationSession> sessions,
        string story,
        bool hasConfession,
        bool hasRefusal)
    {
        var builder = new StringBuilder();

        if (hasConfession)
        {
            builder.AppendLine("Один из допросов дал признание. Суд получил достаточно оснований, чтобы связать показания подозреваемых с уликами дела.");
        }
        else if (hasRefusal)
        {
            builder.AppendLine("Давление в допросе оказалось слишком сильным. Один из подозреваемых отказался говорить, и дело пришлось собирать по неполным показаниям.");
        }
        else
        {
            builder.AppendLine("Оба допроса завершены. Показания подозреваемых сопоставлены с уликами, и материалы переданы в суд.");
        }

        builder.AppendLine();
        builder.AppendLine("Итоги допросов:");
        foreach (var session in sessions)
        {
            builder.AppendLine($"- {session.Suspect.Name}: статус {session.Status}, доверие {session.CurrentTrust}, давление {session.CurrentAggression}.");
        }

        builder.AppendLine();
        builder.AppendLine(story);

        return builder.ToString();
    }
}
