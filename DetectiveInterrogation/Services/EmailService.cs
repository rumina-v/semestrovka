using DetectiveInterrogation.Services.Interfaces;
using DetectiveInterrogation.Settings;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

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

    public async Task SendInterrogationResultEmailAsync(
        string email,
        string username,
        string caseTitle,
        string suspectName,
        string finalStatus,
        IReadOnlyCollection<string> achievements,
        string storyAttachmentText)
    {
        var safeUsername = WebUtility.HtmlEncode(username);
        var safeCaseTitle = WebUtility.HtmlEncode(caseTitle);
        var safeSuspectName = WebUtility.HtmlEncode(suspectName);
        var safeFinalStatus = WebUtility.HtmlEncode(finalStatus);
        var achievementItems = achievements.Count == 0
            ? "<li>No achievements earned yet</li>"
            : string.Join("", achievements.Select(a => $"<li>{WebUtility.HtmlEncode(a)}</li>"));

        var subject = $"Case results: {caseTitle}";
        var body = $"""
            <h1>Case completed</h1>
            <p>Detective {safeUsername}, your case results are ready.</p>
            <p><strong>Case:</strong> {safeCaseTitle}</p>
            <p><strong>Suspect:</strong> {safeSuspectName}</p>
            <p><strong>Final status:</strong> {safeFinalStatus}</p>
            <h2>Achievements</h2>
            <ul>{achievementItems}</ul>
            <p>The full case story is attached as a text file.</p>
            """;

        var fileName = $"{ToFileName(caseTitle)}-full-story.txt";
        await SendEmailWithAttachmentAsync(email, subject, body, storyAttachmentText, fileName);
    }

    private async Task SendEmailWithAttachmentAsync(
        string to,
        string subject,
        string body,
        string attachmentText,
        string attachmentFileName)
    {
        try
        {
            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            var bytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetBytes(attachmentText);
            using var stream = new MemoryStream(bytes);
            using var attachment = new Attachment(stream, attachmentFileName, MediaTypeNames.Text.Plain);
            attachment.ContentType.CharSet = Encoding.UTF8.WebName;
            mailMessage.Attachments.Add(attachment);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email with attachment {AttachmentFileName} sent to {Email}", attachmentFileName, to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachment to {Email}", to);
        }
    }

    private static string ToFileName(string value)
    {
        var fileName = Regex.Replace(value.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(fileName) ? "case" : fileName;
    }
}
