namespace DetectiveInterrogation.Models.ViewModels.Case;

public class CaseListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? NewspaperText { get; set; }
    public int SuspectCount { get; set; }
    public int EvidenceCount { get; set; }
}
