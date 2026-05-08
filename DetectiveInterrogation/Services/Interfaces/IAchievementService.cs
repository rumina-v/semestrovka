using DetectiveInterrogation.Models.DTOs.Achievement;

namespace DetectiveInterrogation.Services.Interfaces;

public interface IAchievementService
{
    Task<List<AchievementDto>> GetAllAchievementsAsync();
    Task<AchievementDto?> GetAchievementByIdAsync(int achievementId);
    Task<List<AchievementDto>> GetUserAchievementsAsync(int userId);
    Task<bool> AwardAchievementAsync(int userId, int achievementId);
    Task<bool> HasAchievementAsync(int userId, int achievementId);
}
