using DetectiveInterrogation.Models.Entities;

namespace DetectiveInterrogation.Services.Interfaces;

public interface IAchievementService
{
    Task<List<Achievement>> GetAllAchievementsAsync();
    Task<Achievement?> GetAchievementByIdAsync(int achievementId);
    Task<List<Achievement>> GetUserAchievementsAsync(int userId);
    Task<bool> AwardAchievementAsync(int userId, int achievementId);
    Task<bool> HasAchievementAsync(int userId, int achievementId);
}
