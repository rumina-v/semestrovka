using DetectiveInterrogation.Models.DTOs.Admin;
using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Repositories.Interfaces;
using DetectiveInterrogation.Services.Interfaces;

namespace DetectiveInterrogation.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IEmailService _emailService;

    public AdminService(
        IUserRepository userRepository,
        IStatisticsRepository statisticsRepository,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _statisticsRepository = statisticsRepository;
        _emailService = emailService;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(ToDto).ToList();
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user == null ? null : ToDto(user);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        return await _userRepository.DeleteAsync(userId);
    }

    public async Task<List<GameStatisticDto>> GetGameStatisticsAsync()
    {
        var stats = new List<GameStatisticDto>
        {
            new() { Label = "Total Users", Value = await _statisticsRepository.GetUsersCountAsync() },
            new() { Label = "Total Cases", Value = await _statisticsRepository.GetCasesCountAsync() },
            new() { Label = "Total Interrogations", Value = await _statisticsRepository.GetInterrogationsCountAsync() }
        };

        return stats;
    }

    public async Task<bool> SendTestEmailAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || string.IsNullOrWhiteSpace(user.Email))
            return false;

        var body = $"""
            <h1>SMTP test</h1>
            <p>Detective Interrogation successfully sent this message through the configured SMTP server.</p>
            <p>User: {System.Net.WebUtility.HtmlEncode(user.Username)}</p>
            <p>Sent at UTC: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}</p>
            """;

        return await _emailService.SendEmailAsync(user.Email, "Detective Interrogation SMTP test", body);
    }

    private static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
    }
}
