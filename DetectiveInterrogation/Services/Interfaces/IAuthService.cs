namespace DetectiveInterrogation.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string? Token, string? Message)> RegisterAsync(string username, string email, string password);
    Task<(bool Success, string? Token, string? Message)> LoginAsync(string username, string password);
    Task<bool> ValidateTokenAsync(string token);
}
