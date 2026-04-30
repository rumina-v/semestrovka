namespace DetectiveInterrogation.Models.ViewModels.Achievement;

public class AchievementViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsUnlocked { get; set; }
}
