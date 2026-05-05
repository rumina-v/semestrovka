using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Repositories.Interfaces;
using DetectiveInterrogation.Services.Interfaces;

namespace DetectiveInterrogation.Services;

public class AchievementService : IAchievementService
{
    private readonly IAchievementRepository _achievementRepository;

    public AchievementService(IAchievementRepository achievementRepository)
    {
        _achievementRepository = achievementRepository;
    }

    public async Task<List<Achievement>> GetAllAchievementsAsync()
    {
        return await _achievementRepository.GetAllAsync();
    }

    public async Task<Achievement?> GetAchievementByIdAsync(int achievementId)
    {
        return await _achievementRepository.GetByIdAsync(achievementId);
    }

    public async Task<List<Achievement>> GetUserAchievementsAsync(int userId)
    {
        return await _achievementRepository.GetByUserIdAsync(userId);
    }

    public async Task<bool> AwardAchievementAsync(int userId, int achievementId)
    {
        if (await _achievementRepository.UserHasAchievementAsync(userId, achievementId))
            return false;

        var userAchievement = new UserAchievement
        {
            UserId = userId,
            AchievementId = achievementId
        };

        await _achievementRepository.AddUserAchievementAsync(userAchievement);
        return true;
    }

    public async Task<bool> HasAchievementAsync(int userId, int achievementId)
    {
        return await _achievementRepository.UserHasAchievementAsync(userId, achievementId);
    }
}
