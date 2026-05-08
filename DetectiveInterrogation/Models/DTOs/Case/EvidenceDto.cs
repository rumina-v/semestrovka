namespace DetectiveInterrogation.Models.DTOs.Case;

public class EvidenceDto
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public string SuspectId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEnumerable<EvidencePhraseDto> Phrases { get; set; } = [];
}
