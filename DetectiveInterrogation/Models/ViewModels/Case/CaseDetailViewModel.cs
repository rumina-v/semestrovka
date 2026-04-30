namespace DetectiveInterrogation.Models.ViewModels.Case;

public class CaseDetailViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? NewspaperText { get; set; }
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
    public List<SuspectListItemViewModel> Suspects { get; set; } = new();
    public List<EvidenceListItemViewModel> Evidence { get; set; } = new();
}

public class SuspectListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int InitialTrust { get; set; }
    public int InitialAggression { get; set; }
    public bool IsGuilty { get; set; }
}

public class EvidenceListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ShortText { get; set; }
    public int PhrasesCount { get; set; }
}
