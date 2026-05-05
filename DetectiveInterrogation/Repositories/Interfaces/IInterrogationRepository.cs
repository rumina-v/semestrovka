using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Models.ViewModels.Interrogation;

namespace DetectiveInterrogation.Repositories.Interfaces;

public interface IInterrogationRepository
{
    Task<Suspect?> GetSuspectByIdAsync(int suspectId);
    Task<InterrogationSession?> GetSessionByIdAsync(int sessionId);
    Task<InterrogationSession?> GetSessionWithCaseAndSuspectAsync(int sessionId);
    Task<InterrogationSession?> GetSessionWithResultDetailsAsync(int sessionId);
    Task<EvidencePhrase?> GetPhraseByIdAsync(int phraseId);
    Task<Evidence?> GetEvidenceByIdAsync(int evidenceId);
    Task<SuspectReply?> GetReplyAsync(int suspectId, int phraseId);
    Task<bool> IsEvidenceUsedAsync(int sessionId, int evidenceId);
    Task<List<int>> GetUsedEvidenceIdsAsync(int sessionId);
    Task<List<AvailablePhraseViewModel>> GetAvailablePhrasesAsync(int suspectId, List<int> usedEvidenceIds);
    Task<List<InterrogationSession>> GetLatestCaseSessionsAsync(int userId, int caseId, int count);
    Task<Achievement?> GetAchievementByTitleAsync(string title);
    Task<List<string>> GetAchievementTitlesByUserIdAsync(int userId);
    Task AddSessionAsync(InterrogationSession session);
    void AddSessionUsedEvidence(SessionUsedEvidence sessionUsedEvidence);
    Task SaveChangesAsync();
}
