namespace DetectiveInterrogation.Models.DTOs.Interrogation;

using System.ComponentModel.DataAnnotations;

public class ProcessPhraseDto
{
    [Range(1, int.MaxValue, ErrorMessage = "PhraseId must be positive")]
    public int PhraseId { get; set; }
}
