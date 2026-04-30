namespace DetectiveInterrogation.Models.Entities;

public class Suspect
{
    public int Id { get; set; }
    
    public int CaseId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public int InitialTrust { get; set; } = 50;
    
    public int InitialAggression { get; set; } = 50;
    
    public bool IsGuilty { get; set; }

    // Navigation properties
    public Case Case { get; set; } = null!;
    public ICollection<SuspectReply> Replies { get; set; } = new List<SuspectReply>();
    public ICollection<InterrogationSession> InterrogationSessions { get; set; } = new List<InterrogationSession>();
}
