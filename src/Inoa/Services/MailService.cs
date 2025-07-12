using System.Data;
using System.Net;
using System.Net.Mail;

using Inoa.Interfaces;

namespace Inoa.Services;

public class MailService : IMailService
{
    private readonly MailAddress _targetEmailAddress;
    private readonly MailAddress _userMailAddress;
    private readonly ISmtpClientWrapper _smtpClient;

    public MailService()
    {
        var smtpHost = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? throw new InvalidOperationException("SMTP_HOST not set.");
        var smtpPort = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : throw new InvalidOperationException("SMTP_PORT not set or invalid.");
        var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? throw new InvalidOperationException("SMTP_USER not set.");
        var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new InvalidOperationException("SMTP_PASSWORD not set.");

        var targetMailAddress = Environment.GetEnvironmentVariable("TARGET_MAIL_ADDRESS")
                                 ?? throw new InvalidOperationException("TARGET_MAIL_ADDRESS not set.");

        _userMailAddress = new MailAddress(smtpUser);
        _targetEmailAddress = new MailAddress(targetMailAddress);

        var smtpClient = new SmtpClient();
        smtpClient.Host = smtpHost;
        smtpClient.Port = smtpPort;
        smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
        smtpClient.EnableSsl = true;

        _smtpClient = new SmtpClientWrapper(smtpClient);
    }

    public MailService(ISmtpClientWrapper smtpClient)
    {
        _smtpClient = smtpClient;

        var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? throw new InvalidOperationException("SMTP_USER not set.");
        var targetMailAddress = Environment.GetEnvironmentVariable("TARGET_MAIL_ADDRESS")
                                 ?? throw new InvalidOperationException("TARGET_MAIL_ADDRESS not set.");

        _userMailAddress = new MailAddress(smtpUser);
        _targetEmailAddress = new MailAddress(targetMailAddress);
    }

    public async Task SendEmailAsync(string subject, string body)
    {
        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentNullException(nameof(subject), "Subject cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(body))
        {
            throw new ArgumentNullException(nameof(body), "Body cannot be null or empty.");
        }

        MailMessage mailMessage = new MailMessage
        {
            From = _userMailAddress,
            Subject = subject,
            Body = body
        };

        mailMessage.To.Add(_targetEmailAddress);

        Console.WriteLine($"[Sending Email] Subject: {subject}, To: {_targetEmailAddress.Address}");

        await _smtpClient.SendMailAsync(mailMessage);
    }
}