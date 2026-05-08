namespace DetectiveInterrogation.Models.DTOs.Case;

public class SuspectDto
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int InitialTrust { get; set; }
    public int InitialPressure { get; set; }
    public bool IsGuilty { get; set; }
}
