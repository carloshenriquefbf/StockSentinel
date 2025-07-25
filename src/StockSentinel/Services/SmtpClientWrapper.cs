using System.Net.Mail;

using StockSentinel.Interfaces;

namespace StockSentinel.Services;

public class SmtpClientWrapper : ISmtpClientWrapper
{
    private readonly SmtpClient _smtpClient;

    public SmtpClientWrapper(SmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public async Task SendMailAsync(MailMessage message)
    {
        await _smtpClient.SendMailAsync(message);
    }
}