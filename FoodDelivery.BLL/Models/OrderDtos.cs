namespace FoodDelivery.BLL.Models;

public sealed class CreateOrderDto
{
    public string RecipientName { get; init; } = string.Empty;
    public string RecipientEmail { get; init; } = string.Empty;
    public string RecipientPhone { get; init; } = string.Empty;
    public string DeliveryAddress { get; init; } = string.Empty;
}

public sealed class OrderListItemDto
{
    public int Id { get; init; }
    public string RestaurantName { get; init; } = string.Empty;
    public string RecipientName { get; init; } = string.Empty;
    public string RecipientEmail { get; init; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; init; }
    public decimal Total { get; init; }
}

public sealed class OrderDetailsDto
{
    public int Id { get; init; }
    public string? UserId { get; init; }
    public string? AccountEmail { get; init; }
    public string RestaurantName { get; init; } = string.Empty;
    public string RecipientName { get; init; } = string.Empty;
    public string RecipientEmail { get; init; } = string.Empty;
    public string RecipientPhone { get; init; } = string.Empty;
    public string DeliveryAddress { get; init; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; init; }
    public decimal Total { get; init; }
    public IReadOnlyList<OrderItemDto> Items { get; init; } = Array.Empty<OrderItemDto>();
}

public sealed class OrderItemDto
{
    public string DishName { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal LineTotal { get; init; }
}
