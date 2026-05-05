using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Repositories.Interfaces;
using DetectiveInterrogation.Services.Interfaces;

namespace DetectiveInterrogation.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IStatisticsRepository _statisticsRepository;

    public AdminService(IUserRepository userRepository, IStatisticsRepository statisticsRepository)
    {
        _userRepository = userRepository;
        _statisticsRepository = statisticsRepository;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        return await _userRepository.DeleteAsync(userId);
    }

    public async Task<List<object>> GetGameStatisticsAsync()
    {
        var stats = new List<object>
        {
            new { Label = "Total Users", Value = await _statisticsRepository.GetUsersCountAsync() },
            new { Label = "Total Cases", Value = await _statisticsRepository.GetCasesCountAsync() },
            new { Label = "Total Interrogations", Value = await _statisticsRepository.GetInterrogationsCountAsync() }
        };

        return stats;
    }
}
