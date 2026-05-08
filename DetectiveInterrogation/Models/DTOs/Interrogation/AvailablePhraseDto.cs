namespace DetectiveInterrogation.Models.DTOs.Interrogation;

public class AvailablePhraseDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string EvidenceTitle { get; set; } = string.Empty;
    public int EvidenceId { get; set; }
}
