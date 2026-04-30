namespace DetectiveInterrogation.Models.ViewModels.Admin;

public class GameStatisticsViewModel
{
    public int TotalUsers { get; set; }
    public int TotalCases { get; set; }
    public int TotalInterrogations { get; set; }
    public int TotalAchievementsAwarded { get; set; }
    public double AverageSessionLength { get; set; }
    public double AveragePlayerSuccess { get; set; }
}
