using System.Diagnostics;
using FoodDelivery.BLL.Interfaces;
using FoodDelivery.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Web.Controllers;

public class HomeController(IRestaurantService restaurantService) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await restaurantService.GetAllAsync());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}