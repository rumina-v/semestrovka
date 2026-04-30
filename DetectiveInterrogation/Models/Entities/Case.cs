namespace DetectiveInterrogation.Models.Entities;

public class Case
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? NewspaperText { get; set; }
    
    public string? ShortDescription { get; set; }
    
    public string? FullDescription { get; set; }

    // Navigation properties
    public ICollection<Suspect> Suspects { get; set; } = new List<Suspect>();
    public ICollection<Evidence> Evidence { get; set; } = new List<Evidence>();
    public ICollection<InterrogationSession> InterrogationSessions { get; set; } = new List<InterrogationSession>();
}
