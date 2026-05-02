using Bookstore.Contracts;
using Dapr.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Contracts;
using UserService.Application.Dtos;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Infrastructure.Repositories;

namespace UserService.API.Controllers;

// Exposes CRUD endpoints for the User Service.
// This controller stays thin and delegates data access to the repository layer.
[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly DaprClient _daprClient;

    public UsersController(IUserRepository repository, DaprClient daprClient)
    {
        _repository = repository;
        _daprClient = daprClient;
    }

    // GET: /api/users
    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _repository.GetAllAsync(cancellationToken);

        var response = users.Select(user => new UserResponse
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            CreatedUtc = user.CreatedUtc
        }).ToList();

        return Ok(response);
    }

    // GET: /api/users/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return NotFound(new { message = $"User with id {id} was not found." });
        }

        return Ok(new UserResponse
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            CreatedUtc = user.CreatedUtc
        });
    }

    //// POST: /api/users
    //[HttpPost]
    //public async Task<ActionResult<UserResponse>> Create(
    //    [FromBody] RegisterUserRequest request,
    //    CancellationToken cancellationToken)
    //{
    //    // Basic validation to keep empty requests out of the database.
    //    if (string.IsNullOrWhiteSpace(request.UserName) ||
    //        string.IsNullOrWhiteSpace(request.Email) ||
    //        string.IsNullOrWhiteSpace(request.Password))
    //    {
    //        return BadRequest(new { message = "UserName, Email, and Password are required." });
    //    }

    //    var normalizedEmail = request.Email.Trim().ToLowerInvariant();

    //    // Prevent duplicate email addresses.
    //    var existingUser = await _repository.GetByEmailAsync(normalizedEmail, cancellationToken);
    //    if (existingUser is not null)
    //    {
    //        return Conflict(new { message = "Email already exists." });
    //    }

    //    // Build the domain entity.
    //    var user = new User
    //    {
    //        UserName = request.UserName.Trim(),
    //        Email = normalizedEmail,
    //        PhoneNumber = request.PhoneNumber,
    //        Role = request.Role,
    //        CreatedUtc = DateTime.UtcNow
    //    };

    //    // Hash the password before storing it.
    //    var passwordHasher = new PasswordHasher<User>();
    //    user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

    //    // Persist the user and capture the generated identity value.
    //    int newUserId = await _repository.AddAsync(user, cancellationToken);
    //    user.UserId = newUserId;

    //    // Publish the integration event after successful persistence.
    //    var createdEvent = new UserRegisteredEvent(
    //        Guid.NewGuid(),
    //        "user.created",
    //        user.UserId,
    //        user.Email,
    //        user.Role.ToString(),
    //        DateTime.UtcNow);

    //    await _daprClient.PublishEventAsync("pubsub", "user.created", createdEvent, cancellationToken);

    //    var response = new UserResponse
    //    {
    //        UserId = user.UserId,
    //        UserName = user.UserName,
    //        Email = user.Email,
    //        PhoneNumber = user.PhoneNumber,
    //        Role = user.Role,
    //        CreatedUtc = user.CreatedUtc
    //    };

    //    return CreatedAtAction(nameof(GetById), new { id = user.UserId }, response);
    //}

    private async Task<IActionResult> RegisterUserInternal(RegisterUserRequest request, UserRole role)
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(request.UserName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "UserName, Email, and Password are required." });
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // Check duplicate email
        var existingUser = await _repository.GetByEmailAsync(normalizedEmail, CancellationToken.None);
        if (existingUser is not null)
        {
            return Conflict(new { message = "Email already exists." });
        }

        // Create user entity
        var user = new User
        {
            UserName = request.UserName.Trim(),
            Email = normalizedEmail,
            PhoneNumber = request.PhoneNumber,
            Role = role,
            CreatedUtc = DateTime.UtcNow
        };

        // HASH PASSWORD 
        var passwordHasher = new PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        // Save to DB
        int userId = await _repository.AddAsync(user, CancellationToken.None);
        user.UserId = userId;

        // Publish event
        var evt = new UserRegisteredEvent(
            Guid.NewGuid(),
            "user.created",
            user.UserId,
            user.Email,
            user.Role.ToString(),
            DateTime.UtcNow
        );

        await _daprClient.PublishEventAsync("pubsub", "user.created", evt);

        return Ok(new
        {
            user.UserId,
            user.UserName,
            user.Email,
            user.PhoneNumber,
            Role = user.Role.ToString(),
            user.CreatedUtc
        });
    }

    // POST: /api/users/register/customer
    [HttpPost("register/customer")]
    public async Task<IActionResult> RegisterCustomer(
        [FromBody] RegisterUserRequest request)
    {
        return await RegisterUserInternal(request, UserRole.Customer);
    }

    // POST: /api/users/register/admin
    [HttpPost("register/admin")]
    public async Task<IActionResult> RegisterAdmin(
        [FromBody] RegisterUserRequest request)
    {
        return await RegisterUserInternal(request, UserRole.Admin);
    }

    // PUT: /api/users/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return NotFound(new { message = $"User with id {id} was not found." });
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // If the email is changing, ensure the new email is not already taken.
        if (!string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
        {
            var emailOwner = await _repository.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (emailOwner is not null && emailOwner.UserId != id)
            {
                return Conflict(new { message = "Email already exists." });
            }
        }

        // Update fields.
        user.UserName = request.UserName.Trim();
        user.Email = normalizedEmail;
        user.PhoneNumber = request.PhoneNumber;
        //user.Role = request.Role;

        // Re-hash the password because the request includes password data.
        var passwordHasher = new PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        bool updated = await _repository.UpdateAsync(user, cancellationToken);

        if (!updated)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "User update failed." });
        }

        return NoContent();
    }

    // DELETE: /api/users/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return NotFound(new { message = $"User with id {id} was not found." });
        }

        await _repository.DeleteAsync(id, cancellationToken);

        // Publish the deletion event so downstream services can react asynchronously.
        var deletedEvent = new UserDeletedEvent(
            Guid.NewGuid(),
            "user.deleted",
            id,
            DateTime.UtcNow);

        await _daprClient.PublishEventAsync("pubsub", "user.deleted", deletedEvent, cancellationToken);

        return NoContent();
    }
}
