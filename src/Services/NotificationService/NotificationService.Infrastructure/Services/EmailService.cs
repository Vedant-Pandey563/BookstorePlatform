using System.Net;
using System.Net.Mail;
using NotificationService.Application.Contracts;
namespace NotificationService.Infrastructure.Services;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        // Configure SMTP client 
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587, // TLS port
            Credentials = new NetworkCredential(
                "vpemail.02@gmail.com",       // gmail mail
                "kbbl kbdt vmsh szuw"           // Gmail App Password
            ),
            EnableSsl = true
        };

        // Create email message
        var mailMessage = new MailMessage
        {
            From = new MailAddress("vpemail.02@gmail.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        // Add receiver
        mailMessage.To.Add(toEmail);

        // Send email asynchronously
        await smtpClient.SendMailAsync(mailMessage);

        // Log to console for debugging
        Console.WriteLine($"[EMAIL SENT] to {toEmail}");
    }
}