using FoodDelivery.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.BLL.Interfaces;
    public interface IDishService
    {
        Task<ServiceResult> CreateAsync(CreateDishDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }

