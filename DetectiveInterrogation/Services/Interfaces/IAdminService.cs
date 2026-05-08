using DetectiveInterrogation.Models.DTOs.Admin;

namespace DetectiveInterrogation.Services.Interfaces;

public interface IAdminService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<bool> DeleteUserAsync(int userId);
    Task<List<GameStatisticDto>> GetGameStatisticsAsync();
    Task<bool> SendTestEmailAsync(int userId);
}
