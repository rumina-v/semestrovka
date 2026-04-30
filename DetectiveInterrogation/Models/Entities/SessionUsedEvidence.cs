namespace DetectiveInterrogation.Models.Entities;

public class SessionUsedEvidence
{
    public int Id { get; set; }
    
    public int EvidenceId { get; set; }
    
    public int SessionId { get; set; }

    // Navigation properties
    public Evidence Evidence { get; set; } = null!;
    public InterrogationSession Session { get; set; } = null!;
}
