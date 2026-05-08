namespace DetectiveInterrogation.Models.DTOs.Interrogation;

public class CaseEndingDto
{
    public int CaseId { get; set; }
    public string CaseTitle { get; set; } = string.Empty;
    public string ResultTitle { get; set; } = string.Empty;
    public string ResultText { get; set; } = string.Empty;
    public string? CourtImagePath { get; set; }
    public string? PrisonImagePath { get; set; }
    public int AverageTrust { get; set; }
    public int AveragePressure { get; set; }
    public IEnumerable<EndingSessionDto> Sessions { get; set; } = [];
    public IEnumerable<string> Achievements { get; set; } = [];
}

public class EndingSessionDto
{
    public int Id { get; set; }
    public string SuspectName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int CurrentTrust { get; set; }
    public int CurrentPressure { get; set; }
}
