using DetectiveInterrogation.Data;
using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DetectiveInterrogation.Services;

public class AchievementService : IAchievementService
{
    private readonly AppDbContext _context;

    public AchievementService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Achievement>> GetAllAchievementsAsync()
    {
        return await _context.Achievements.ToListAsync();
    }

    public async Task<Achievement?> GetAchievementByIdAsync(int achievementId)
    {
        return await _context.Achievements.FindAsync(achievementId);
    }

    public async Task<List<Achievement>> GetUserAchievementsAsync(int userId)
    {
        return await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.Achievement)
            .ToListAsync();
    }

    public async Task<bool> AwardAchievementAsync(int userId, int achievementId)
    {
        if (await _context.UserAchievements.AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId))
            return false; // Already awarded

        var userAchievement = new UserAchievement
        {
            UserId = userId,
            AchievementId = achievementId
        };

        _context.UserAchievements.Add(userAchievement);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasAchievementAsync(int userId, int achievementId)
    {
        return await _context.UserAchievements.AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);
    }
}
