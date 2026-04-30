namespace DetectiveInterrogation.Services.Interfaces;

public interface IExternalApiService
{
    Task<object?> CallExternalApiAsync(string endpoint, object? data = null);
    Task<bool> ValidateExternalDataAsync(object data);
}
