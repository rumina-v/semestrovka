using DetectiveInterrogation.Models.DTOs.External;

namespace DetectiveInterrogation.Services.Interfaces;

public interface IExternalApiService
{
    Task<ExternalApiResponseDto> CallExternalApiAsync(string endpoint, string? jsonPayload = null);
    Task<bool> ValidateExternalDataAsync(object data);
}
