namespace DetectiveInterrogation.Models.DTOs.External;

public class ExternalApiResponseDto
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Content { get; set; }
    public string? ErrorMessage { get; set; }
}
