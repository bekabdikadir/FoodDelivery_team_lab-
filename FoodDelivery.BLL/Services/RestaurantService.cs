using FoodDelivery.BLL.Interfaces;
using FoodDelivery.BLL.Models;
using FoodDelivery.DAL.Data;
using FoodDelivery.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.BLL.Services;

public class RestaurantService(ApplicationDbContext db) : IRestaurantService
{
    public async Task<List<RestaurantDto>> GetAllAsync()
    {
        return await db.Restaurants
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new RestaurantDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                ImageUrl = r.ImageUrl,
                DishCount = r.Dishes.Count
            })
            .ToListAsync();
    }

    public async Task<RestaurantDetailsDto?> GetByIdAsync(int id)
    {
        return await db.Restaurants
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new RestaurantDetailsDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                ImageUrl = r.ImageUrl,
                DishCount = r.Dishes.Count,
                Dishes = r.Dishes
                    .OrderBy(d => d.Name)
                    .Select(d => new DishDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Description = d.Description,
                        Price = d.Price,
                        RestaurantId = d.RestaurantId
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateRestaurantDto dto)
    {
        var name = dto.Name.Trim();
        if (name.Length == 0)
        {
            return ServiceResult<int>.Failure("Укажите название заведения");
        }

        var restaurant = new Restaurant
        {
            Name = name,
            Description = dto.Description.Trim(),
            ImageUrl = dto.ImageUrl
        };

        db.Restaurants.Add(restaurant);
        await db.SaveChangesAsync();

        return ServiceResult<int>.Success(restaurant.Id);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var restaurant = await db.Restaurants.FindAsync(id);
        if (restaurant == null)
        {
            return ServiceResult.Failure("Заведение не найдено");
        }

        db.Restaurants.Remove(restaurant);
        await db.SaveChangesAsync();

        return ServiceResult.Success();
    }
}