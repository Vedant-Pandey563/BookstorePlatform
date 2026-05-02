using UserService.Domain.Enums;

namespace UserService.Domain.Entities;

// Represents a user record stored in SQL Server.
// This entity contains only user-profile data and stays inside the Domain layer.
public sealed class User
{
    // SQL Server identity primary key.
    public int UserId { get; set; }

    // Display name shown in the application.
    public string UserName { get; set; } = string.Empty;

    // Unique email address.
    public string Email { get; set; } = string.Empty;

    // Phone number stored as long to match the assignment contract.
    public long PhoneNumber { get; set; }

    // Hashed password only. Never store the raw password.
    public string PasswordHash { get; set; } = string.Empty;

    // Role used for authorization and business logic.
    public UserRole Role { get; set; } = UserRole.Customer;

    // Record creation timestamp in UTC.
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
