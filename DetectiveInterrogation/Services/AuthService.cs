using DetectiveInterrogation.Data;
using DetectiveInterrogation.Helpers;
using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DetectiveInterrogation.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtTokenHelper _jwtTokenHelper;

    public AuthService(AppDbContext context, PasswordHasher passwordHasher, JwtTokenHelper jwtTokenHelper)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenHelper = jwtTokenHelper;
    }

    public async Task<(bool Success, string? Token, string? Message)> RegisterAsync(string username, string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username))
            return (false, null, "Username already exists");

        if (await _context.Users.AnyAsync(u => u.Email == email))
            return (false, null, "Email already exists");

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(password),
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtTokenHelper.GenerateToken(user.Id, user.Username, user.Email);
        return (true, token, "Registration successful");
    }

    public async Task<(bool Success, string? Token, string? Message)> LoginAsync(string username, string password)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        if (user == null)
            return (false, null, "Invalid username or password");

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            return (false, null, "Invalid username or password");

        var token = _jwtTokenHelper.GenerateToken(user.Id, user.Username, user.Email);
        return (true, token, "Login successful");
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        return _jwtTokenHelper.ValidateToken(token) != null;
    }
}
