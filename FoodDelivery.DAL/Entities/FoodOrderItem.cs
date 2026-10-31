namespace FoodDelivery.DAL.Entities;

public class FoodOrderItem
{
    public int Id { get; set; }
    public int FoodOrderId { get; set; }
    public FoodOrder? FoodOrder { get; set; }
    public string DishName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
