namespace Bookstore.Contracts;

//  event payload published when a user registers.
public sealed record UserRegisteredEvent(
    Guid EventId,
    string EventType,
    int UserId,
    string Email,
    string Role,
    DateTime OccurredAtUtc);
