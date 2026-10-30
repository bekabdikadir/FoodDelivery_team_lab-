using FoodDelivery.BLL.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodDelivery.BLL.Interfaces;

public interface IRestaurantService
{
    Task<List<RestaurantDto>> GetAllAsync();
    Task<RestaurantDetailsDto?> GetByIdAsync(int id);
    Task<ServiceResult<int>> CreateAsync(CreateRestaurantDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
