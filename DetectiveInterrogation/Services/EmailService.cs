using DetectiveInterrogation.Services.Interfaces;
using DetectiveInterrogation.Settings;
using System.Net;
using System.Net.Mail;

namespace DetectiveInterrogation.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(EmailSettings emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent to {Email}", to);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
        }
    }

    public async Task SendWelcomeEmailAsync(string email, string username)
    {
        var subject = "Welcome to Detective Interrogation";
        var body = $"<h1>Welcome, {username}!</h1><p>Thank you for joining our detective game.</p>";
        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        var subject = "Password Reset";
        var body = $"<p><a href='{resetLink}'>Click here to reset your password</a></p>";
        await SendEmailAsync(email, subject, body);
    }
}
