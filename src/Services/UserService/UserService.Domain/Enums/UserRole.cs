namespace UserService.Domain.Enums;

// Represents the role assigned to a user inside the User Service boundary.
// Keep the enum here in the Domain layer because it is a core business concept.
public enum UserRole
{
    Customer = 0,
    Admin = 1,
}
