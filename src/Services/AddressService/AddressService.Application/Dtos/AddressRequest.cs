using AddressService.Domain.Enums;

namespace AddressService.Application.Dtos;

public sealed class AddressRequest
{
    public string Name { get; set; } = string.Empty;
    public long MobileNumber { get; set; }
    public string UserAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public AddressType Type { get; set; }
    public int UserId { get; set; }
}
