using DetectiveInterrogation.Models.DTOs.External;
using DetectiveInterrogation.Services.Interfaces;
using DetectiveInterrogation.Settings;

namespace DetectiveInterrogation.Services;

public class ExternalApiService : IExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly ExternalApiSettings _settings;
    private readonly ILogger<ExternalApiService> _logger;

    public ExternalApiService(HttpClient httpClient, ExternalApiSettings settings, ILogger<ExternalApiService> logger)
    {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
    }

    public async Task<ExternalApiResponseDto> CallExternalApiAsync(string endpoint, string? jsonPayload = null)
    {
        try
        {
            var url = $"{_settings.BaseUrl}{endpoint}";
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.Timeout);
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);

            HttpResponseMessage response;
            if (!string.IsNullOrWhiteSpace(jsonPayload))
            {
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync(url, content);
            }
            else
            {
                response = await _httpClient.GetAsync(url);
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return new ExternalApiResponseDto
                {
                    Success = true,
                    StatusCode = (int)response.StatusCode,
                    Content = responseContent
                };
            }

            _logger.LogWarning("External API call failed: {StatusCode}", response.StatusCode);
            return new ExternalApiResponseDto
            {
                Success = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = "External API call failed"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling external API");
            return new ExternalApiResponseDto
            {
                Success = false,
                StatusCode = 0,
                ErrorMessage = "Error calling external API"
            };
        }
    }

    public async Task<bool> ValidateExternalDataAsync(object data)
    {
        return await Task.FromResult(true);
    }
}
