namespace DetectiveInterrogation.Models.DTOs.Case;

public class CaseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? NewspaperText { get; set; }
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
    public IEnumerable<SuspectDto> Suspects { get; set; } = [];
    public IEnumerable<EvidenceDto> Evidence { get; set; } = [];
}
