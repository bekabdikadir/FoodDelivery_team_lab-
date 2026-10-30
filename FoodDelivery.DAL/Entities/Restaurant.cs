namespace FoodDelivery.DAL.Entities;

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
   
}
