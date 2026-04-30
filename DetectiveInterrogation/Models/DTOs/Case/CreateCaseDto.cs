namespace DetectiveInterrogation.Models.DTOs.Case;

public class CreateCaseDto
{
    public string Title { get; set; } = string.Empty;
    public string? NewspaperText { get; set; }
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
}
