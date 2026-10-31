using FoodDelivery.BLL.Interfaces;
using FoodDelivery.BLL.Models;
using FoodDelivery.DAL.Data;
using FoodDelivery.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.BLL.Services;

public sealed class OrderService(ApplicationDbContext db) : IOrderService
{
    public async Task<ServiceResult<int>> PlaceAsync(
        int restaurantId,
        IReadOnlyDictionary<int, int> quantities,
        CreateOrderDto request,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var recipientName = request.RecipientName.Trim();
        var recipientEmail = request.RecipientEmail.Trim();
        var recipientPhone = request.RecipientPhone.Trim();
        var deliveryAddress = request.DeliveryAddress.Trim();

        if (recipientName.Length is < 2 or > 200 || recipientEmail.Length is < 3 or > 256 ||
            recipientPhone.Length is < 5 or > 40 || deliveryAddress.Length is < 5 or > 1000)
        {
            return ServiceResult<int>.Failure("Проверьте контактные данные и адрес доставки.");
        }

        if (quantities.Count == 0 || quantities.Any(line => line.Key <= 0 || line.Value is < 1 or > 99))
        {
            return ServiceResult<int>.Failure("Корзина пуста или содержит некорректное количество блюд.");
        }

        var restaurant = await db.Restaurants.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == restaurantId, cancellationToken);
        if (restaurant is null)
        {
            return ServiceResult<int>.Failure("Заведение не найдено.");
        }

        if (!string.IsNullOrWhiteSpace(userId) &&
            !await db.Users.AnyAsync(user => user.Id == userId, cancellationToken))
        {
            return ServiceResult<int>.Failure("Пользователь не найден. Войдите в аккаунт повторно.");
        }

        var dishIds = quantities.Keys.ToArray();
        var dishes = await db.Dishes.AsNoTracking()
            .Where(item => item.RestaurantId == restaurantId && dishIds.Contains(item.Id))
            .ToListAsync(cancellationToken);
        if (dishes.Count != quantities.Count)
        {
            return ServiceResult<int>.Failure("Одно из блюд больше недоступно. Обновите корзину.");
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var order = new FoodOrder
        {
            UserId = string.IsNullOrWhiteSpace(userId) ? null : userId,
            RestaurantId = restaurantId,
            RestaurantName = restaurant.Name,
            RecipientName = recipientName,
            RecipientEmail = recipientEmail,
            RecipientPhone = recipientPhone,
            DeliveryAddress = deliveryAddress,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            Items = dishes.Select(dish => new FoodOrderItem
            {
                DishName = dish.Name,
                UnitPrice = dish.Price,
                Quantity = quantities[dish.Id],
                LineTotal = dish.Price * quantities[dish.Id]
            }).ToList()
        };
        order.Total = order.Items.Sum(item => item.LineTotal);

        db.FoodOrders.Add(order);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ServiceResult<int>.Success(order.Id);
    }

    public Task<List<OrderListItemDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        db.FoodOrders.AsNoTracking()
            .OrderByDescending(order => order.CreatedAtUtc)
            .Select(order => new OrderListItemDto
            {
                Id = order.Id,
                RestaurantName = order.RestaurantName,
                RecipientName = order.RecipientName,
                RecipientEmail = order.RecipientEmail,
                CreatedAtUtc = order.CreatedAtUtc,
                Total = order.Total
            })
            .ToListAsync(cancellationToken);

    public Task<OrderDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.FoodOrders.AsNoTracking()
            .Where(order => order.Id == id)
            .Select(order => new OrderDetailsDto
            {
                Id = order.Id,
                UserId = order.UserId,
                AccountEmail = order.User == null ? null : order.User.Email,
                RestaurantName = order.RestaurantName,
                RecipientName = order.RecipientName,
                RecipientEmail = order.RecipientEmail,
                RecipientPhone = order.RecipientPhone,
                DeliveryAddress = order.DeliveryAddress,
                CreatedAtUtc = order.CreatedAtUtc,
                Total = order.Total,
                Items = order.Items.OrderBy(item => item.Id).Select(item => new OrderItemDto
                {
                    DishName = item.DishName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    LineTotal = item.LineTotal
                }).ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);
}
