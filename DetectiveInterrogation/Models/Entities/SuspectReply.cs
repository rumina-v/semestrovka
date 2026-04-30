namespace DetectiveInterrogation.Models.Entities;

public class SuspectReply
{
    public int Id { get; set; }
    
    public int SuspectId { get; set; }
    
    public int PhraseId { get; set; }
    
    public string ReplyText { get; set; } = string.Empty;
    
    public int TrustChange { get; set; } = 0;
    
    public int AggressionChange { get; set; } = 0;

    // Navigation properties
    public Suspect Suspect { get; set; } = null!;
    public EvidencePhrase Phrase { get; set; } = null!;
}
