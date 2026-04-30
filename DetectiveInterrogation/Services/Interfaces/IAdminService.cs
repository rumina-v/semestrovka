using DetectiveInterrogation.Models.Entities;

namespace DetectiveInterrogation.Services.Interfaces;

public interface IAdminService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<bool> DeleteUserAsync(int userId);
    Task<List<object>> GetGameStatisticsAsync();
}
