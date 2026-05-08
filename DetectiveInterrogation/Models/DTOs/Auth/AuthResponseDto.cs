namespace DetectiveInterrogation.Models.DTOs.Auth;

public class AuthResponseDto
{
    public string Message { get; set; } = string.Empty;
    public AuthUserDto? User { get; set; }
}

public class AuthUserDto
{
    public int? Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
}
