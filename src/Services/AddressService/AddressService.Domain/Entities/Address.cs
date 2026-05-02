using AddressService.Domain.Enums;

namespace AddressService.Domain.Entities;

// Represents an address record owned by Address Service.
// IMPORTANT: Only UserId is stored, no navigation to User (microservice rule).
public sealed class Address
{
    public int AddressId { get; set; }

    public string Name { get; set; } = string.Empty;

    public long MobileNumber { get; set; }

    public string UserAddress { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public AddressType Type { get; set; }

    public int UserId { get; set; }
}
