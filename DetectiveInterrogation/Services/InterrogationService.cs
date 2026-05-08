using DetectiveInterrogation.Models.DTOs.Interrogation;
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

    public async Task<StartInterrogationResponseDto?> StartInterrogationSessionAsync(int userId, int caseId, int suspectId)
    {
        try
        {
            var suspect = await _interrogationRepository.GetSuspectByIdAsync(suspectId);
            if (suspect == null)
                return null;

            if (suspect.CaseId != caseId)
                return null;

            var session = new InterrogationSession
            {
                UserId = userId,
                CaseId = caseId,
                SuspectId = suspectId,
                CurrentTrust = suspect.InitialTrust,
                CurrentPressure = suspect.InitialPressure,
                Status = "InProgress"
            };

            await _interrogationRepository.AddSessionAsync(session);

            _logger.LogInformation("Interrogation session started for user {UserId}, suspect {SuspectId}", userId, suspectId);

            return new StartInterrogationResponseDto
            {
                Id = session.Id,
                CurrentTrust = session.CurrentTrust,
                CurrentPressure = session.CurrentPressure,
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

    public async Task<InterrogationResultDto?> ProcessPhrasSelectionAsync(int userId, int sessionId, int phraseId)
    {
        try
        {
            var session = await _interrogationRepository.GetSessionByIdAsync(sessionId);
            if (session == null)
                return null;

            if (session.UserId != userId)
                return null;

            var phrase = await _interrogationRepository.GetPhraseByIdAsync(phraseId);
            if (phrase == null)
                return null;

            var evidence = await _interrogationRepository.GetEvidenceByIdAsync(phrase.EvidenceId);
            if (evidence == null)
                return null;

            if (!IsPhraseAllowedForSession(session, evidence))
                return new InterrogationResultDto { Error = "This phrase is not available for this interrogation session" };

            var alreadyUsed = await _interrogationRepository.IsEvidenceUsedAsync(sessionId, evidence.Id);

            if (alreadyUsed)
                return new InterrogationResultDto { Error = "This evidence has already been used in this interrogation session" };

            var sessionUsedEvidence = new SessionUsedEvidence
            {
                SessionId = sessionId,
                EvidenceId = evidence.Id
            };
            _interrogationRepository.AddSessionUsedEvidence(sessionUsedEvidence);

            session.CurrentTrust = Math.Clamp(session.CurrentTrust + phrase.TrustChange, 0, 100);
            session.CurrentPressure = Math.Clamp(session.CurrentPressure + phrase.PressureChange, 0, 100);

            var reply = await _interrogationRepository.GetReplyAsync(
                session.SuspectId,
                phraseId,
                session.CurrentTrust,
                session.CurrentPressure);

            if (reply == null)
                return new InterrogationResultDto { Error = "No reply found for current trust and pressure" };

            CheckInterrogationEnd(session);

            await _interrogationRepository.SaveChangesAsync();

            _logger.LogInformation("Phrase processed in session {SessionId}: Trust={Trust}, Pressure={Pressure}",
                sessionId, session.CurrentTrust, session.CurrentPressure);

            return new InterrogationResultDto
            {
                PhraseText = phrase.Text,
                ReplyText = reply.Text,
                CurrentTrust = session.CurrentTrust,
                CurrentPressure = session.CurrentPressure,
                TrustChange = phrase.TrustChange,
                PressureChange = phrase.PressureChange,
                Status = session.Status,
                EvidenceTitle = evidence.Title
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing phrase selection");
            return null;
        }
    }

    public async Task<InterrogationStateDto?> GetSessionStateAsync(int userId, int sessionId)
    {
        try
        {
            var session = await _interrogationRepository.GetSessionWithCaseAndSuspectAsync(sessionId);

            if (session == null)
                return null;

            if (session.UserId != userId)
                return null;

            var usedEvidenceIds = await _interrogationRepository.GetUsedEvidenceIdsAsync(sessionId);

            return new InterrogationStateDto
            {
                Id = session.Id,
                CurrentTrust = session.CurrentTrust,
                CurrentPressure = session.CurrentPressure,
                Status = session.Status,
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

    public async Task<bool> EndInterrogationSessionAsync(int userId, int sessionId)
    {
        try
        {
            var session = await _interrogationRepository.GetSessionWithResultDetailsAsync(sessionId);
            if (session == null)
                return false;

            if (session.UserId != userId)
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

    public async Task<List<AvailablePhraseDto>> GetAvailablePhrasesAsync(int userId, int sessionId)
    {
        try
        {
            var session = await _interrogationRepository.GetSessionByIdAsync(sessionId);
            if (session == null)
                return new List<AvailablePhraseDto>();

            if (session.UserId != userId)
                return new List<AvailablePhraseDto>();

            var usedEvidenceIds = await _interrogationRepository.GetUsedEvidenceIdsAsync(sessionId);

            var availablePhrases = await _interrogationRepository.GetAvailablePhrasesAsync(session.SuspectId, usedEvidenceIds);

            return availablePhrases;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available phrases");
            return new List<AvailablePhraseDto>();
        }
    }

    public async Task<bool> AwardAchievementAsync(int userId, int achievementId)
    {
        return await _achievementService.AwardAchievementAsync(userId, achievementId);
    }

    public async Task<CaseEndingDto?> GetCaseEndingAsync(int userId, int caseId)
    {
        try
        {
            var sessions = await _interrogationRepository.GetLatestCaseSessionsAsync(userId, caseId, 2);
            if (sessions.Count == 0)
                return null;

            sessions = sessions.OrderBy(s => s.Id).ToList();
            var caseEntity = sessions.Last().Case;
            var achievements = await _interrogationRepository.GetAchievementTitlesByUserIdAsync(userId);
            var hasConfession = sessions.Any(s => s.Status == "Confession");
            var hasRefusal = sessions.Any(s => s.Status == "Refused");
            var averageTrust = (int)Math.Round(sessions.Average(s => s.CurrentTrust));
            var averagePressure = (int)Math.Round(sessions.Average(s => s.CurrentPressure));

            var resultTitle = hasConfession
                ? "Признание получено"
                : hasRefusal
                    ? "Допрос сорвался"
                    : "Расследование завершено";

            var resultText = SelectEndingText(caseEntity, hasConfession, hasRefusal);

            return new CaseEndingDto
            {
                CaseId = caseId,
                CaseTitle = caseEntity.Title,
                ResultTitle = resultTitle,
                ResultText = resultText,
                CourtImagePath = caseEntity.CourtImagePath,
                PrisonImagePath = caseEntity.PrisonImagePath,
                AverageTrust = averageTrust,
                AveragePressure = averagePressure,
                Sessions = sessions.Select(s => new EndingSessionDto
                {
                    Id = s.Id,
                    SuspectName = s.Suspect.Name,
                    Status = s.Status,
                    CurrentTrust = s.CurrentTrust,
                    CurrentPressure = s.CurrentPressure
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
        if (session.CurrentTrust == 100 && session.CurrentPressure == 100)
        {
            session.Status = "Confession";
            _logger.LogInformation("Suspect confessed in session {SessionId}", session.Id);
        }
        else if (session.CurrentTrust == 0 && session.CurrentPressure == 100)
        {
            session.Status = "Refused";
            _logger.LogInformation("Suspect refused to talk in session {SessionId}", session.Id);
        }
    }

    private static bool IsPhraseAllowedForSession(InterrogationSession session, Evidence evidence)
    {
        return evidence.CaseId == session.CaseId
            && IsEvidenceAssignedToSuspect(evidence, session.SuspectId);
    }

    private static bool IsEvidenceAssignedToSuspect(Evidence evidence, int suspectId)
    {
        var suspectIdText = suspectId.ToString();

        return evidence.SuspectId
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Contains(suspectIdText);
    }

    private static string SelectEndingText(Case caseEntity, bool hasConfession, bool hasRefusal)
    {
        var text = hasConfession
            ? caseEntity.EndingSuccessText
            : hasRefusal
                ? caseEntity.EndingRefusalText
                : caseEntity.EndingDefaultText;

        return string.IsNullOrWhiteSpace(text)
            ? "Расследование завершено. Материалы дела переданы в суд, а итоговое решение вынесено по собранным уликам и показаниям."
            : text;
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
            builder.AppendLine($"- {session.Suspect.Name}: статус {session.Status}, доверие {session.CurrentTrust}, давление {session.CurrentPressure}.");
        }

        builder.AppendLine();
        builder.AppendLine(story);

        return builder.ToString();
    }
}
