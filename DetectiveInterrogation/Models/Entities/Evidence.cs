namespace DetectiveInterrogation.Models.Entities;

public class Evidence
{
    public int Id { get; set; }
    
    public int CaseId { get; set; }

    public string SuspectId { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public Case Case { get; set; } = null!;
    public ICollection<EvidencePhrase> Phrases { get; set; } = new List<EvidencePhrase>();
    public ICollection<SessionUsedEvidence> SessionUsedEvidences { get; set; } = new List<SessionUsedEvidence>();
}
