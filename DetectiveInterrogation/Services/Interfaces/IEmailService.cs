namespace DetectiveInterrogation.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendWelcomeEmailAsync(string email, string username);
    Task SendPasswordResetEmailAsync(string email, string resetLink);
}
