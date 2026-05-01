namespace DetectiveInterrogation.Models.Entities;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    // Navigation properties
    public ICollection<InterrogationSession> InterrogationSessions { get; set; } = new List<InterrogationSession>();

    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}
