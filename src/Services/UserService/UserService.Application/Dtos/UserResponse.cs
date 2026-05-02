using UserService.Domain.Enums;

namespace UserService.Application.Dtos;

// Response model returned by the API.
// This excludes PasswordHash because it must never be exposed.
public sealed class UserResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public long PhoneNumber { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedUtc { get; set; }
}
