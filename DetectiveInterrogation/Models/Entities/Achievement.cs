namespace DetectiveInterrogation.Models.Entities;

public class Achievement
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }

    // Navigation properties
    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}
