namespace DetectiveInterrogation.Models.ViewModels.Interrogation;

public class InterrogationSessionViewModel
{
    public int SessionId { get; set; }
    public string SuspectName { get; set; } = string.Empty;
    public string CaseName { get; set; } = string.Empty;
    public int CurrentTrust { get; set; }
    public int CurrentAggression { get; set; }
    public string Status { get; set; } = "InProgress";
    public List<AvailablePhraseViewModel> AvailablePhrases { get; set; } = new();
    public string? LastReply { get; set; }
}
