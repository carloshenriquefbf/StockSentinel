namespace Inoa.Interfaces;

public interface IMailService
{
    public Task SendEmailAsync(string subject, string body);
}