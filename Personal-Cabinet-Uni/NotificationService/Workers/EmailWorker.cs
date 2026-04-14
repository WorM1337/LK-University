using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Personal_Cabinet_Uni.Shared.Models.DTO.Request;

namespace NotificationService.Workers;

public interface IEmailWorker
{
    Task SendEmailAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}

public class EmailWorker : IEmailWorker
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailWorker> _logger;

    public EmailWorker(IOptions<EmailSettings> emailSettings, ILogger<EmailWorker> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.From));
            email.To.Add(MailboxAddress.Parse(message.To));
            email.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.IsHtml ? message.Body : null,
                TextBody = message.IsHtml ? null : message.Body
            };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls, cancellationToken);
            await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password, cancellationToken);
            await smtp.SendAsync(email, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent to {To} with subject {Subject}", message.To, message.Subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", message.To);
            throw;
        }
    }
}

public class EmailSettings
{
    public string SmtpServer { get; set; } = "localhost";
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
}
