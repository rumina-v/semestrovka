using DetectiveInterrogation.Models.DTOs.Auth;

namespace DetectiveInterrogation.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(string username, string email, string password);
    Task<AuthResultDto> LoginAsync(string username, string password);
    Task<bool> ValidateTokenAsync(string token);
}
