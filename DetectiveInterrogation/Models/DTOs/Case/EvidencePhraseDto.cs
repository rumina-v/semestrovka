namespace DetectiveInterrogation.Models.DTOs.Case;

public class EvidencePhraseDto
{
    public int Id { get; set; }
    public int EvidenceId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int TrustChange { get; set; }
    public int PressureChange { get; set; }
}
