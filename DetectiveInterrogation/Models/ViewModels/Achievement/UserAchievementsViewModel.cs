namespace DetectiveInterrogation.Models.ViewModels.Achievement;

public class UserAchievementsViewModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int TotalAchievements { get; set; }
    public int UnlockedCount { get; set; }
    public List<AchievementViewModel> Achievements { get; set; } = new();
}
