using DetectiveInterrogation.Services.Interfaces;
using DetectiveInterrogation.Settings;
using System.Text.Json;

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

    public async Task<object?> CallExternalApiAsync(string endpoint, object? data = null)
    {
        try
        {
            var url = $"{_settings.BaseUrl}{endpoint}";
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.Timeout);
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);

            HttpResponseMessage response;
            if (data != null)
            {
                var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync(url, content);
            }
            else
            {
                response = await _httpClient.GetAsync(url);
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<object>(responseContent);
            }

            _logger.LogWarning("External API call failed: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling external API");
            return null;
        }
    }

    public async Task<bool> ValidateExternalDataAsync(object data)
    {
        // Implement custom validation logic here
        return await Task.FromResult(true);
    }
}
