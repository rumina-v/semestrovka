namespace DetectiveInterrogation.Models.Entities;

public class Evidence
{
    public int Id { get; set; }
    
    public int CaseId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? ShortText { get; set; }
    
    public string? FullText { get; set; }

    // Navigation properties
    public Case Case { get; set; } = null!;
    public ICollection<EvidencePhrase> Phrases { get; set; } = new List<EvidencePhrase>();
    public ICollection<SessionUsedEvidence> SessionUsedEvidences { get; set; } = new List<SessionUsedEvidence>();
}
