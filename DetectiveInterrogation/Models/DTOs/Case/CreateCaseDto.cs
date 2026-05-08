namespace DetectiveInterrogation.Models.DTOs.Case;

using System.ComponentModel.DataAnnotations;

public class CreateCaseDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 255 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(4000, ErrorMessage = "Newspaper text is too long")]
    public string? NewspaperText { get; set; }

    [StringLength(4000, ErrorMessage = "Short description is too long")]
    public string? ShortDescription { get; set; }

    [StringLength(20000, ErrorMessage = "Full description is too long")]
    public string? FullDescription { get; set; }
}
