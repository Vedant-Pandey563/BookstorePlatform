namespace Bookstore.Contracts;

// event payload published when a user is deleted.
public sealed record UserDeletedEvent(
    Guid EventId,
    string EventType,
    int UserId,
    DateTime OccurredAtUtc);
