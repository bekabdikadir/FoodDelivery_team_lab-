using FoodDelivery.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Web.Controllers;
public class RestaurantsController(IRestaurantService restaurantService) : Controller
{
    public async Task<IActionResult> Details(int id)
    {
        var restaurant = await restaurantService.GetByIdAsync(id);
        if (restaurant == null)
        {
            return NotFound();
        }

        return View(restaurant);
    }
}
