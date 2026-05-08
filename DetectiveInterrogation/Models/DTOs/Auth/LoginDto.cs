namespace DetectiveInterrogation.Models.DTOs.Auth;

using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [RegularExpression(@"^[A-Za-z0-9_.-]+$", ErrorMessage = "Username can contain only letters, digits, dots, dashes and underscores")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    public string Password { get; set; } = string.Empty;
}
