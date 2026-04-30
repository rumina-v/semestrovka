using DetectiveInterrogation.Data;
using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DetectiveInterrogation.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.InterrogationSessions)
            .Include(u => u.UserAchievements)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<object>> GetGameStatisticsAsync()
    {
        var stats = new List<object>
        {
            new { Label = "Total Users", Value = await _context.Users.CountAsync() },
            new { Label = "Total Cases", Value = await _context.Cases.CountAsync() },
            new { Label = "Total Interrogations", Value = await _context.InterrogationSessions.CountAsync() }
        };

        return stats;
    }
}
