using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Web.Models;
public class RestaurantCreateViewModel
{
    [Required(ErrorMessage = "Укажите название")]
    [StringLength(200)]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    [Display(Name = "Описание")]
    public string? Description { get; set; }

    [Display(Name = "Изображение")]
    public IFormFile? Image { get; set; }
}

public class DishCreateViewModel
{
    public int RestaurantId { get; set; }

    public string RestaurantName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите название")]
    [StringLength(200)]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    [Display(Name = "Описание")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Укажите цену")]
    [Range(0.01, 1000000, ErrorMessage = "Цена должна быть больше нуля")]
    [Display(Name = "Цена")]
    public decimal? Price { get; set; }
}
