namespace DetectiveInterrogation.Models.ViewModels.Interrogation;

public class SuspectReplyViewModel
{
    public string ReplyText { get; set; } = string.Empty;
    public int NewTrust { get; set; }
    public int NewAggression { get; set; }
    public int TrustChange { get; set; }
    public int AggressionChange { get; set; }
    public string EvidenceTitle { get; set; } = string.Empty;
    public string? Message { get; set; }
}
