namespace DetectiveInterrogation.Models.Entities;

public class User
{
    public const string AdminRole = "Admin";
    public const string PlayerRole = "Player";

    public int Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = PlayerRole;

    public ICollection<InterrogationSession> InterrogationSessions { get; set; } = new List<InterrogationSession>();
    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}
