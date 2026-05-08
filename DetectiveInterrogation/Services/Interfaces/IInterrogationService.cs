using DetectiveInterrogation.Models.DTOs.Interrogation;

namespace DetectiveInterrogation.Services.Interfaces;

public interface IInterrogationService
{
    Task<StartInterrogationResponseDto?> StartInterrogationSessionAsync(int userId, int caseId, int suspectId);
    Task<InterrogationResultDto?> ProcessPhrasSelectionAsync(int userId, int sessionId, int phraseId);
    Task<InterrogationStateDto?> GetSessionStateAsync(int userId, int sessionId);
    Task<CaseEndingDto?> GetCaseEndingAsync(int userId, int caseId);
    Task<bool> EndInterrogationSessionAsync(int userId, int sessionId);
    Task<List<AvailablePhraseDto>> GetAvailablePhrasesAsync(int userId, int sessionId);
    Task<bool> AwardAchievementAsync(int userId, int achievementId);
}
