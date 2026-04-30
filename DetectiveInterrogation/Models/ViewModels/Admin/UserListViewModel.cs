namespace DetectiveInterrogation.Models.ViewModels.Admin;

public class UserListViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int InterrogationCount { get; set; }
    public int AchievementCount { get; set; }
}
