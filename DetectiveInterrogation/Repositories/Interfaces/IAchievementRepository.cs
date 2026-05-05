using DetectiveInterrogation.Models.Entities;

namespace DetectiveInterrogation.Repositories.Interfaces;

public interface IAchievementRepository
{
    Task<List<Achievement>> GetAllAsync();
    Task<Achievement?> GetByIdAsync(int achievementId);
    Task<List<Achievement>> GetByUserIdAsync(int userId);
    Task<bool> UserHasAchievementAsync(int userId, int achievementId);
    Task AddUserAchievementAsync(UserAchievement userAchievement);
}
