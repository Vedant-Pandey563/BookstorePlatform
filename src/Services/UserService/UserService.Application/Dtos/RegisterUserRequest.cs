using UserService.Domain.Enums;

namespace UserService.Application.Dtos;

// Request model used by the API when creating or updating a user.
// This stays in Application because it is part of the service contract.
public sealed class RegisterUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public long PhoneNumber { get; set; }
    public string Password { get; set; } = string.Empty;
    //public UserRole Role { get; set; } = UserRole.Customer;
}
