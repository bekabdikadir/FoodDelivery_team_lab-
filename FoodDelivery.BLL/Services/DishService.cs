using FoodDelivery.BLL.Interfaces;
using FoodDelivery.BLL.Models;
using FoodDelivery.DAL.Data;
using FoodDelivery.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.BLL.Services;

public class DishService(ApplicationDbContext db) : IDishService
{
    public async Task<ServiceResult> CreateAsync(CreateDishDto dto)
    {
        if (!await db.Restaurants.AnyAsync(r => r.Id == dto.RestaurantId))
        {
            return ServiceResult.Failure("Заведение не найдено");
        }

        var name = dto.Name.Trim();
        if (name.Length == 0)
        {
            return ServiceResult.Failure("Укажите название блюда");
        }

        if (dto.Price <= 0)
        {
            return ServiceResult.Failure("Цена должна быть больше нуля");
        }

        db.Dishes.Add(new Dish
        {
            RestaurantId = dto.RestaurantId,
            Name = name,
            Description = dto.Description.Trim(),
            Price = dto.Price
        });
        await db.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var dish = await db.Dishes.FindAsync(id);
        if (dish == null)
        {
            return ServiceResult.Failure("Блюдо не найдено");
        }

        db.Dishes.Remove(dish);
        await db.SaveChangesAsync();

        return ServiceResult.Success();
    }
}