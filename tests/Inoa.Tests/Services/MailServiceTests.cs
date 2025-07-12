using System.Net.Mail;

using Inoa.Interfaces;
using Inoa.Services;

using Moq;

using Xunit;

namespace Inoa.Tests.Services;

public class MailServiceTests
{
    private readonly Mock<ISmtpClientWrapper> _mockSmtpClient;
    private readonly MailService _mailService;

    public MailServiceTests()
    {
        Environment.SetEnvironmentVariable("SMTP_SERVER", "smtp.test.com");
        Environment.SetEnvironmentVariable("SMTP_PORT", "587");
        Environment.SetEnvironmentVariable("SMTP_USER", "test@gmail.com");
        Environment.SetEnvironmentVariable("SMTP_PASSWORD", "test-password");
        Environment.SetEnvironmentVariable("TARGET_MAIL_ADDRESS", "target@gmail.com");

        _mockSmtpClient = new Mock<ISmtpClientWrapper>();
        _mailService = new MailService(_mockSmtpClient.Object);
    }

    [Fact]
    public async Task SendEmailAsync_WithValidParameters_SendsEmailSuccessfully()
    {
        string subject = "Test Subject";
        string body = "Test Body";

        MailMessage? capturedMessage = null;

        _mockSmtpClient
            .Setup(x => x.SendMailAsync(It.IsAny<MailMessage>()))
            .Callback<MailMessage>(msg => capturedMessage = msg)
            .Returns(Task.CompletedTask)
            .Verifiable();

        await _mailService.SendEmailAsync(subject, body);

        _mockSmtpClient.Verify(x => x.SendMailAsync(It.IsAny<MailMessage>()), Times.Once);

        Assert.NotNull(capturedMessage);
        Assert.Equal(subject, capturedMessage.Subject);
        Assert.Equal(body, capturedMessage.Body);
    }

    [Fact]
    public async Task SendEmailAsync_NullSubject_ThrowsArgumentNullException()
    {
        string? subject = null;
        string body = "Test Body";

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _mailService.SendEmailAsync(subject!, body));

        Assert.Equal("subject", exception.ParamName);
    }

    [Fact]
    public async Task SendEmailAsync_NullBody_ThrowsArgumentNullException()
    {
        string subject = "Test Subject";
        string? body = null;

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _mailService.SendEmailAsync(subject, body!));

        Assert.Equal("body", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithoutSmtpServer_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("SMTP_SERVER", null);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            new MailService());

        Assert.Equal("SMTP_HOST not set.", exception.Message);

        Environment.SetEnvironmentVariable("SMTP_SERVER", "smtp.test.com");
    }

    [Fact]
    public void Constructor_WithoutSmtpPort_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("SMTP_PORT", null);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            new MailService());

        Assert.Equal("SMTP_PORT not set or invalid.", exception.Message);

        Environment.SetEnvironmentVariable("SMTP_PORT", "587");
    }

    [Fact]
    public void Constructor_WithoutSmtpUser_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("SMTP_USER", null);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            new MailService());

        Assert.Equal("SMTP_USER not set.", exception.Message);

        Environment.SetEnvironmentVariable("SMTP_USER", "test@example.com");
    }

    [Fact]
    public void Constructor_WithoutSmtpPassword_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("SMTP_PASSWORD", null);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            new MailService());

        Assert.Equal("SMTP_PASSWORD not set.", exception.Message);

        Environment.SetEnvironmentVariable("SMTP_PASSWORD", "test-password");
    }

    [Fact]
    public void Constructor_WithoutTargetEmailAccount_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("TARGET_MAIL_ADDRESS", null);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            new MailService());

        Assert.Equal("TARGET_MAIL_ADDRESS not set.", exception.Message);

        Environment.SetEnvironmentVariable("TARGET_MAIL_ADDRESS", "target@example.com");
    }

    [Fact]
    public void Constructor_WithValidEnvironmentVariables_CreatesInstanceSuccessfully()
    {
        Environment.SetEnvironmentVariable("SMTP_SERVER", "smtp.test.com");
        Environment.SetEnvironmentVariable("SMTP_PORT", "587");
        Environment.SetEnvironmentVariable("SMTP_USER", "test@example.com");
        Environment.SetEnvironmentVariable("SMTP_PASSWORD", "test-password");
        Environment.SetEnvironmentVariable("TARGET_MAIL_ADDRESS", "target@example.com");

        var mailService = new MailService();

        Assert.NotNull(mailService);
    }
}