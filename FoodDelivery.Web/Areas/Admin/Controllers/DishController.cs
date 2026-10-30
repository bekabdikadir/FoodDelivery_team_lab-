using FoodDelivery.BLL;
using FoodDelivery.BLL.Interfaces;
using FoodDelivery.BLL.Models;
using FoodDelivery.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class DishesController(IRestaurantService restaurantService, IDishService dishService) : Controller
{
    public async Task<IActionResult> Index(int restaurantId)
    {
        var restaurant = await restaurantService.GetByIdAsync(restaurantId);
        if (restaurant == null)
        {
            return NotFound();
        }

        return View(restaurant);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int restaurantId)
    {
        var restaurant = await restaurantService.GetByIdAsync(restaurantId);
        if (restaurant == null)
        {
            return NotFound();
        }

        return View(new DishCreateViewModel
        {
            RestaurantId = restaurant.Id,
            RestaurantName = restaurant.Name
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DishCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await dishService.CreateAsync(new CreateDishDto
        {
            RestaurantId = model.RestaurantId,
            Name = model.Name,
            Description = model.Description ?? string.Empty,
            Price = model.Price!.Value
        });

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        TempData["Success"] = "Блюдо добавлено";
        return RedirectToAction(nameof(Index), new { restaurantId = model.RestaurantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int restaurantId)
    {
        var result = await dishService.DeleteAsync(id);
        if (result.Succeeded)
        {
            TempData["Success"] = "Блюдо удалено";
        }
        else
        {
            TempData["Error"] = string.Join(". ", result.Errors);
        }

        return RedirectToAction(nameof(Index), new { restaurantId });
    }
}