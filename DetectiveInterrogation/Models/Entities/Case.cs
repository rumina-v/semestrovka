namespace DetectiveInterrogation.Models.Entities;

public class Case
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? NewspaperText { get; set; }
    
    public string? ShortDescription { get; set; }
    
    public string? FullDescription { get; set; }

    public string? EndingSuccessText { get; set; }

    public string? EndingRefusalText { get; set; }

    public string? EndingDefaultText { get; set; }

    public string? CourtImagePath { get; set; }

    public string? PrisonImagePath { get; set; }

    public ICollection<Suspect> Suspects { get; set; } = new List<Suspect>();
    public ICollection<Evidence> Evidence { get; set; } = new List<Evidence>();
    public ICollection<InterrogationSession> InterrogationSessions { get; set; } = new List<InterrogationSession>();
}
