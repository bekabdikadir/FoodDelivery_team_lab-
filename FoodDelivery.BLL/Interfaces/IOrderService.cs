using FoodDelivery.BLL.Models;

namespace FoodDelivery.BLL.Interfaces;

public interface IOrderService
{
    Task<ServiceResult<int>> PlaceAsync(
        int restaurantId,
        IReadOnlyDictionary<int, int> quantities,
        CreateOrderDto order,
        string? userId,
        CancellationToken cancellationToken = default);

    Task<List<OrderListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
