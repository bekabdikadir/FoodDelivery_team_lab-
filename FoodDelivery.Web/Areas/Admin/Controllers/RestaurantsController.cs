using FoodDelivery.BLL;
using FoodDelivery.BLL.Interfaces;
using FoodDelivery.BLL.Models;
using FoodDelivery.Web.Models;
using FoodDelivery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class RestaurantsController(IRestaurantService restaurantService, IImageStorage imageStorage) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await restaurantService.GetAllAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new RestaurantCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RestaurantCreateViewModel model)
    {
        if (model.Image is { Length: > 0 } && !imageStorage.IsValid(model.Image))
        {
            ModelState.AddModelError(nameof(model.Image), "Допустимы изображения JPG, PNG, GIF, WEBP размером до 5 МБ");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string? imageUrl = null;
        if (model.Image is { Length: > 0 })
        {
            imageUrl = await imageStorage.SaveAsync(model.Image, "restaurants");
        }

        var result = await restaurantService.CreateAsync(new CreateRestaurantDto
        {
            Name = model.Name,
            Description = model.Description ?? string.Empty,
            ImageUrl = imageUrl
        });

        if (!result.Succeeded)
        {
            imageStorage.Delete(imageUrl);  
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        TempData["Success"] = "Заведение добавлено";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var restaurant = await restaurantService.GetByIdAsync(id);
        if (restaurant == null)
        {
            return NotFound();
        }

        var result = await restaurantService.DeleteAsync(id);
        if (result.Succeeded)
        {
            imageStorage.Delete(restaurant.ImageUrl);
            TempData["Success"] = "Заведение удалено";
        }
        else
        {
            TempData["Error"] = string.Join(". ", result.Errors);
        }

        return RedirectToAction(nameof(Index));
    }
}