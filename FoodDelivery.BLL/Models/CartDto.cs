namespace FoodDelivery.BLL.Models;

public sealed class CartLineDto
{
    public int DishId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public sealed class RestaurantCartDto
{
    public int RestaurantId { get; init; }
    public string RestaurantName { get; init; } = string.Empty;
    public IReadOnlyList<CartLineDto> Items { get; init; } = Array.Empty<CartLineDto>();
    public decimal Total => Items.Sum(item => item.LineTotal);
    public int ItemCount => Items.Sum(item => item.Quantity);
}
