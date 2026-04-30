namespace DetectiveInterrogation.Models.ViewModels.Interrogation;

public class InterrogationResultViewModel
{
    public int SessionId { get; set; }
    public string SuspectName { get; set; } = string.Empty;
    public string FinalStatus { get; set; } = string.Empty; // Confession, Refused, Neutral
    public int FinalTrust { get; set; }
    public int FinalAggression { get; set; }
    public string ConclusionText { get; set; } = string.Empty;
    public List<string> AwardedAchievements { get; set; } = new();
}

