using AddressService.Domain.Entities;

namespace AddressService.Application.Contracts;

public interface IAddressRepository
{
    Task<List<Address>> GetAllAsync();
    Task<Address?> GetByIdAsync(int id);
    Task<List<Address>> GetByUserIdAsync(int userId);
    Task<int> AddAsync(Address address);
    Task<bool> UpdateAsync(Address address);
    Task DeleteAsync(int id);
    Task DeleteByUserIdAsync(int userId);
}
