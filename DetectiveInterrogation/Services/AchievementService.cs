using DetectiveInterrogation.Models.DTOs.Achievement;
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

    public async Task<List<AchievementDto>> GetAllAchievementsAsync()
    {
        var achievements = await _achievementRepository.GetAllAsync();
        return achievements.Select(ToDto).ToList();
    }

    public async Task<AchievementDto?> GetAchievementByIdAsync(int achievementId)
    {
        var achievement = await _achievementRepository.GetByIdAsync(achievementId);
        return achievement == null ? null : ToDto(achievement);
    }

    public async Task<List<AchievementDto>> GetUserAchievementsAsync(int userId)
    {
        var achievements = await _achievementRepository.GetByUserIdAsync(userId);
        return achievements.Select(ToDto).ToList();
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

    private static AchievementDto ToDto(Achievement achievement)
    {
        return new AchievementDto
        {
            Id = achievement.Id,
            Title = achievement.Title,
            Description = achievement.Description
        };
    }
}
