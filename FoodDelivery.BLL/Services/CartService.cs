using FoodDelivery.BLL.Interfaces;
using FoodDelivery.BLL.Models;
using FoodDelivery.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.BLL.Services;

public sealed class CartService(ApplicationDbContext db) : ICartService
{
    public async Task<ServiceResult<RestaurantCartDto>> GetRestaurantCartAsync(
        int restaurantId,
        IReadOnlyDictionary<int, int> quantities,
        CancellationToken cancellationToken = default)
    {
        var restaurant = await db.Restaurants.AsNoTracking()
            .Where(item => item.Id == restaurantId)
            .Select(item => new { item.Id, item.Name })
            .SingleOrDefaultAsync(cancellationToken);

        if (restaurant is null)
        {
            return ServiceResult<RestaurantCartDto>.Failure("Заведение не найдено.");
        }

        if (quantities.Count == 0)
        {
            return ServiceResult<RestaurantCartDto>.Success(new RestaurantCartDto
            {
                RestaurantId = restaurant.Id,
                RestaurantName = restaurant.Name
            });
        }

        if (quantities.Any(item => item.Key <= 0 || item.Value is < 1 or > 99))
        {
            return ServiceResult<RestaurantCartDto>.Failure("Количество блюда должно быть от 1 до 99 порций.");
        }

        var dishIds = quantities.Keys.ToArray();
        var dishes = await db.Dishes.AsNoTracking()
            .Where(item => item.RestaurantId == restaurantId && dishIds.Contains(item.Id))
            .OrderBy(item => item.Name)
            .Select(item => new { item.Id, item.Name, item.Price })
            .ToListAsync(cancellationToken);

        if (dishes.Count != quantities.Count)
        {
            return ServiceResult<RestaurantCartDto>.Failure("Одно из блюд больше недоступно в выбранном заведении. Обновите корзину.");
        }

        var lines = dishes.Select(dish => new CartLineDto
        {
            DishId = dish.Id,
            Name = dish.Name,
            UnitPrice = dish.Price,
            Quantity = quantities[dish.Id]
        }).ToList();

        return ServiceResult<RestaurantCartDto>.Success(new RestaurantCartDto
        {
            RestaurantId = restaurant.Id,
            RestaurantName = restaurant.Name,
            Items = lines
        });
    }
}
