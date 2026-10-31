namespace FoodDelivery.DAL.Entities;

public class FoodOrder
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public decimal Total { get; set; }
    public ICollection<FoodOrderItem> Items { get; set; } = new List<FoodOrderItem>();
}
