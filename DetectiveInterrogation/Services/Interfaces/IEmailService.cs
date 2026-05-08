namespace DetectiveInterrogation.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body);
    Task<bool> SendWelcomeEmailAsync(string email, string username);
    Task<bool> SendPasswordResetEmailAsync(string email, string resetLink);
    Task<bool> SendInterrogationResultEmailAsync(
        string email,
        string username,
        string caseTitle,
        string suspectName,
        string finalStatus,
        IReadOnlyCollection<string> achievements,
        string storyAttachmentText);
}
