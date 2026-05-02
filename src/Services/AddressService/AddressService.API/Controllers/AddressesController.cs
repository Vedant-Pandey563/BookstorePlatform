using AddressService.Application.Contracts;
using AddressService.Application.Dtos;
using AddressService.Domain.Entities;
using Bookstore.Contracts;
using Dapr;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/addresses")]
public class AddressesController : ControllerBase
{
    private readonly IAddressRepository _repo;

    public AddressesController(IAddressRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _repo.GetAllAsync();
        return Ok(data);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        return Ok(await _repo.GetByUserIdAsync(userId));
    }

    [HttpPost]
    public async Task<IActionResult> Create(AddressRequest req)
    {
        var address = new Address
        {
            Name = req.Name,
            MobileNumber = req.MobileNumber,
            UserAddress = req.UserAddress,
            City = req.City,
            State = req.State,
            Type = req.Type,
            UserId = req.UserId
        };

        var id = await _repo.AddAsync(address);
        return Ok(id);
    }

    //  Dapr Subscription
    [Topic("pubsub", "user.deleted")]
    [HttpPost("user-deleted")]
    public async Task<IActionResult> HandleUserDeleted(UserDeletedEvent evt)
    {
        Console.WriteLine($"[EVENT RECEIVED] user.deleted for UserId: {evt.UserId}");

        await _repo.DeleteByUserIdAsync(evt.UserId);

        Console.WriteLine($"[ACTION DONE] Deleted addresses for UserId: {evt.UserId}");

        return Ok();
    }
}
