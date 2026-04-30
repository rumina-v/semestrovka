namespace DetectiveInterrogation.Models.Entities;

public class UserAchievement
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public int AchievementId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Achievement Achievement { get; set; } = null!;
}
