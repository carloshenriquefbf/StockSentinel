using System.Net.Mail;

namespace Inoa.Interfaces;

public interface ISmtpClientWrapper
{
    Task SendMailAsync(MailMessage message);
}