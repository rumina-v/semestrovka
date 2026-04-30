namespace DetectiveInterrogation.Models.Entities;

public class InterrogationSession
{
    public int Id { get; set; }
    
    public int CaseId { get; set; }
    
    public int SuspectId { get; set; }
    
    public int UserId { get; set; }
    
    public int CurrentTrust { get; set; } = 50;
    
    public int CurrentAggression { get; set; } = 50;
    
    public string Status { get; set; } = "InProgress";

    // Navigation properties
    public Case Case { get; set; } = null!;
    public Suspect Suspect { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<SessionUsedEvidence> SessionUsedEvidences { get; set; } = new List<SessionUsedEvidence>();
}
