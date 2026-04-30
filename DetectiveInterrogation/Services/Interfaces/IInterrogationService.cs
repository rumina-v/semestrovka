namespace DetectiveInterrogation.Services.Interfaces;

public interface IInterrogationService
{
    Task<object?> StartInterrogationSessionAsync(int userId, int caseId, int suspectId);
    Task<object?> ProcessPhrasSelectionAsync(int sessionId, int phraseId);
    Task<object?> GetSessionStateAsync(int sessionId);
    Task<bool> EndInterrogationSessionAsync(int sessionId);
    Task<List<object>> GetAvailablePhrasesAsync(int sessionId);
    Task<bool> AwardAchievementAsync(int userId, int achievementId);
}
