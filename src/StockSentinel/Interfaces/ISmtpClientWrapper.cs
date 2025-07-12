using System.Net.Mail;

namespace StockSentinel.Interfaces;

public interface ISmtpClientWrapper
{
    Task SendMailAsync(MailMessage message);
}