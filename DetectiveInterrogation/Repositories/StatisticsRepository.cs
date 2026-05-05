using DetectiveInterrogation.Data;
using DetectiveInterrogation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DetectiveInterrogation.Repositories;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly AppDbContext _context;

    public StatisticsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetUsersCountAsync()
    {
        return await _context.Users.CountAsync();
    }

    public async Task<int> GetCasesCountAsync()
    {
        return await _context.Cases.CountAsync();
    }

    public async Task<int> GetInterrogationsCountAsync()
    {
        return await _context.InterrogationSessions.CountAsync();
    }
}
