using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.Features.Orders.Commands.CancelOrder;
using OrderService.Application.Features.Orders.Commands.CreateOrder;
using OrderService.Application.Features.Orders.Commands.CreateOrderFromCart;
using OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;
using OrderService.Application.Features.Orders.Queries.GetAllOrders;
using OrderService.Application.Features.Orders.Queries.GetOrderById;
using OrderService.Application.Features.Orders.Queries.GetOrderItems;
using OrderService.Application.Features.Orders.Queries.GetOrdersByStatus;
using OrderService.Application.Features.Orders.Queries.GetOrdersByUserId;
using OrderService.Domain.Enums;

namespace OrderService.API.Controllers;

// Thin controller: no business logic here.
// All rules stay in the command/query handlers and the domain rules layer.
[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Customer can create an order.
    [HttpPost]
    //[Authorize]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.OrderId }, result);
    }

    // Optional route for cart checkout flows.
    [HttpPost("from-cart")]
    [Authorize]
    public async Task<ActionResult<OrderDto>> CreateFromCart([FromBody] CreateOrderFromCartCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.OrderId }, result);
    }

    // Customer can see one order.
    [HttpGet("{id:int}")]
    //[Authorize]
    public async Task<ActionResult<OrderDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    // Customer can see their own orders.
    [HttpGet("user/{userId:int}")]
    //[Authorize]
    public async Task<ActionResult<List<OrderDto>>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrdersByUserIdQuery(userId), cancellationToken);
        return Ok(result);
    }

    // Admin can see all orders.
    [HttpGet]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllOrdersQuery(), cancellationToken);
        return Ok(result);
    }

    // Filter by status.
    [HttpGet("status/{status}")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<OrderDto>>> GetByStatus(OrderStatus status, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrdersByStatusQuery(status), cancellationToken);
        return Ok(result);
    }

    // Admin can update order status.
    [HttpPut("{id:int}/status")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(int id, [FromBody] UpdateOrderStatusCommand command, CancellationToken cancellationToken)
    {
        if (id != command.OrderId)
        {
            return BadRequest("Route id and body OrderId must match.");
        }

        var result = await _mediator.Send(command, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    // Cancel order when allowed by business rules.
    [HttpPut("{id:int}/cancel")]
    //[Authorize]
    public async Task<ActionResult<OrderDto>> Cancel(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelOrderCommand(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    // Retrieve order items separately.
    [HttpGet("{id:int}/items")]
    [Authorize]
    public async Task<ActionResult<List<OrderItemDto>>> GetItems(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrderItemsQuery(id), cancellationToken);
        return Ok(result);
    }
}
