using DetectiveInterrogation.Data;
using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DetectiveInterrogation.Repositories;

public class AchievementRepository : IAchievementRepository
{
    private readonly AppDbContext _context;

    public AchievementRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Achievement>> GetAllAsync()
    {
        return await _context.Achievements.ToListAsync();
    }

    public async Task<Achievement?> GetByIdAsync(int achievementId)
    {
        return await _context.Achievements.FindAsync(achievementId);
    }

    public async Task<List<Achievement>> GetByUserIdAsync(int userId)
    {
        return await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.Achievement)
            .ToListAsync();
    }

    public async Task<bool> UserHasAchievementAsync(int userId, int achievementId)
    {
        return await _context.UserAchievements.AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);
    }

    public async Task AddUserAchievementAsync(UserAchievement userAchievement)
    {
        _context.UserAchievements.Add(userAchievement);
        await _context.SaveChangesAsync();
    }
}
