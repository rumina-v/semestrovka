namespace DetectiveInterrogation.Models.ViewModels.Auth;

public class AuthResponseViewModel
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserInfoViewModel? User { get; set; }
}

public class UserInfoViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
