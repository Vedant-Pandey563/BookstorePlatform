namespace OrderService.Application.DTOs;

// Request item used by create-order commands.
public class CreateOrderItemDto
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
