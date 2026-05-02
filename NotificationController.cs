using Bookstore.Contracts;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Contracts;

namespace NotificationService.API.Controllers;

// Controller handling events and sending notifications
[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly IEmailService _emailService;

    // Inject EmailService
    public NotificationController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    // DAPR SUBSCRIPTION
    //  listens to "user.created" events from pubsub
    [Topic("pubsub", "user.created")]
    [HttpPost("user-created")]
    public async Task<IActionResult> HandleUserCreated(UserRegisteredEvent evt)
    {
        // Log full event
        Console.WriteLine($"[EVENT RECEIVED] user.created for {evt.Email}");
        Console.WriteLine($"UserId: {evt.UserId}");
        Console.WriteLine($"Email: {evt.Email}");
        Console.WriteLine($"Role: {evt.Role}");
        Console.WriteLine($"EventId: {evt.EventId}");

        // Send email
        await _emailService.SendEmailAsync(
            evt.Email,
            "Welcome to Bookstore",
            $"Hello User {evt.UserId},\n\n" +
            $"Your account has been successfully created.\n" +
            $"Role: {evt.Role}\n\n" +
            $"Thank you!"
        );

        Console.WriteLine("[NOTIFICATION COMPLETED]");

        return Ok();
    }
}