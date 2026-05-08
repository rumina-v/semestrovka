namespace DetectiveInterrogation.Models.DTOs.Interrogation;

using System.ComponentModel.DataAnnotations;

public class StartInterrogationDto
{
    [Range(1, int.MaxValue, ErrorMessage = "CaseId must be positive")]
    public int CaseId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "SuspectId must be positive")]
    public int SuspectId { get; set; }
}
