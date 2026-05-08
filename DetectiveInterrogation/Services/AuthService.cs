using DetectiveInterrogation.Helpers;
using DetectiveInterrogation.Models.DTOs.Auth;
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

    public async Task<AuthResultDto> RegisterAsync(string username, string email, string password)
    {
        if (await _userRepository.ExistsByUsernameAsync(username))
            return new AuthResultDto { Success = false, Message = "Username already exists" };

        if (await _userRepository.ExistsByEmailAsync(email))
            return new AuthResultDto { Success = false, Message = "Email already exists" };

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(password),
            Role = User.PlayerRole
        };

        await _userRepository.AddAsync(user);

        var token = _jwtTokenHelper.GenerateToken(user.Id, user.Username, user.Email, user.Role);
        return new AuthResultDto { Success = true, Token = token, Message = "Registration successful" };
    }

    public async Task<AuthResultDto> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            return new AuthResultDto { Success = false, Message = "Invalid username or password" };

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            return new AuthResultDto { Success = false, Message = "Invalid username or password" };

        var token = _jwtTokenHelper.GenerateToken(user.Id, user.Username, user.Email, user.Role);
        return new AuthResultDto { Success = true, Token = token, Message = "Login successful" };
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        return Task.FromResult(_jwtTokenHelper.ValidateToken(token) != null);
    }
}
