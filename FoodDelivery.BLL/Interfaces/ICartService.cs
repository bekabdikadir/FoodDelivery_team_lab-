using FoodDelivery.BLL.Models;

namespace FoodDelivery.BLL.Interfaces;

public interface ICartService
{
    Task<ServiceResult<RestaurantCartDto>> GetRestaurantCartAsync(
        int restaurantId,
        IReadOnlyDictionary<int, int> quantities,
        CancellationToken cancellationToken = default);
}
