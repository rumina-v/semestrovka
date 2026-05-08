namespace DetectiveInterrogation.Models.DTOs.Interrogation;

public class InterrogationResultDto
{
    public string? Error { get; set; }
    public string PhraseText { get; set; } = string.Empty;
    public string ReplyText { get; set; } = string.Empty;
    public int CurrentTrust { get; set; }
    public int CurrentPressure { get; set; }
    public int TrustChange { get; set; }
    public int PressureChange { get; set; }
    public string Status { get; set; } = string.Empty;
    public string EvidenceTitle { get; set; } = string.Empty;
}
