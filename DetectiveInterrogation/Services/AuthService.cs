using DetectiveInterrogation.Helpers;
using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Repositories.Interfaces;
using DetectiveInterrogation.Services.Interfaces;

namespace DetectiveInterrogation.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtTokenHelper _jwtTokenHelper;

    public AuthService(IUserRepository userRepository, PasswordHasher passwordHasher, JwtTokenHelper jwtTokenHelper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenHelper = jwtTokenHelper;
    }

    public async Task<(bool Success, string? Token, string? Message)> RegisterAsync(string username, string email, string password)
    {
        if (await _userRepository.ExistsByUsernameAsync(username))
            return (false, null, "Username already exists");

        if (await _userRepository.ExistsByEmailAsync(email))
            return (false, null, "Email already exists");

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(password),
            Role = User.PlayerRole
        };

        await _userRepository.AddAsync(user);

        var token = _jwtTokenHelper.GenerateToken(user.Id, user.Username, user.Email, user.Role);
        return (true, token, "Registration successful");
    }

    public async Task<(bool Success, string? Token, string? Message)> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            return (false, null, "Invalid username or password");

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            return (false, null, "Invalid username or password");

        var token = _jwtTokenHelper.GenerateToken(user.Id, user.Username, user.Email, user.Role);
        return (true, token, "Login successful");
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        return Task.FromResult(_jwtTokenHelper.ValidateToken(token) != null);
    }
}
