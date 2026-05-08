namespace DetectiveInterrogation.Models.DTOs.Interrogation;

public class InterrogationStateDto
{
    public int Id { get; set; }
    public int CurrentTrust { get; set; }
    public int CurrentPressure { get; set; }
    public string Status { get; set; } = string.Empty;
    public string SuspectName { get; set; } = string.Empty;
    public string CaseName { get; set; } = string.Empty;
    public int UsedEvidenceCount { get; set; }
}
