namespace DetectiveInterrogation.Models.Entities;

public class EvidencePhrase
{
    public int Id { get; set; }
    
    public int EvidenceId { get; set; }
    
    public string Text { get; set; } = string.Empty;

    public int TrustChange { get; set; }

    public int PressureChange { get; set; }

    public Evidence Evidence { get; set; } = null!;
    public ICollection<SuspectReply> SuspectReplies { get; set; } = new List<SuspectReply>();
}
