using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Domain.Rules;

// Central business rules for order lifecycle and validation.
public static class OrderRules
{
    public static void EnsureHasItems(IReadOnlyCollection<OrderItem> items)
    {
        if (items is null || items.Count == 0)
        {
            throw new InvalidOperationException("An order must contain at least one item.");
        }
    }

    public static void EnsureValidItemQuantities(IEnumerable<OrderItem> items)
    {
        if (items.Any(x => x.Quantity <= 0))
        {
            throw new InvalidOperationException("Quantity must be greater than zero for every order item.");
        }

        if (items.Any(x => x.UnitPrice <= 0))
        {
            throw new InvalidOperationException("Unit price must be greater than zero for every order item.");
        }
    }

    public static decimal CalculateTotal(IEnumerable<OrderItem> items)
    {
        return items.Sum(x => x.LineTotal);
    }

    public static bool CanCancel(OrderStatus currentStatus)
    {
        // Business policy: once shipped or delivered, cancellation is not allowed.
        return currentStatus is not OrderStatus.Shipped and not OrderStatus.Delivered and not OrderStatus.Cancelled;
    }

    public static bool CanTransition(OrderStatus from, OrderStatus to)
    {
        // Keep the lifecycle explicit and controlled.
        return from switch
        {
            OrderStatus.Pending => to is OrderStatus.Confirmed or OrderStatus.PaymentPending or OrderStatus.Cancelled,
            OrderStatus.Confirmed => to is OrderStatus.PaymentPending or OrderStatus.Paid or OrderStatus.Cancelled,
            OrderStatus.PaymentPending => to is OrderStatus.Paid or OrderStatus.Cancelled,
            OrderStatus.Paid => to is OrderStatus.Shipped or OrderStatus.Cancelled,
            OrderStatus.Shipped => to is OrderStatus.Delivered,
            OrderStatus.Delivered => false,
            OrderStatus.Cancelled => false,
            _ => false
        };
    }
}
