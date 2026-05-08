namespace DetectiveInterrogation.Models.Entities;

public class SuspectReply
{
    public int Id { get; set; }
    
    public int SuspectId { get; set; }
    
    public int EvidencePhraseId { get; set; }
    
    public int MinTrust { get; set; }

    public int MaxTrust { get; set; }

    public int MinPressure { get; set; }

    public int MaxPressure { get; set; }

    public string Text { get; set; } = string.Empty;

    public Suspect Suspect { get; set; } = null!;
    public EvidencePhrase Phrase { get; set; } = null!;
}
