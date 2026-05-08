namespace DetectiveInterrogation.Models.DTOs.Interrogation;

public class StartInterrogationResponseDto
{
    public int Id { get; set; }
    public int CurrentTrust { get; set; }
    public int CurrentPressure { get; set; }
    public string SuspectName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
