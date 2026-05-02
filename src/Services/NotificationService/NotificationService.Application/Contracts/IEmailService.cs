namespace NotificationService.Application.Contracts;

// Contract for sending emails
public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}