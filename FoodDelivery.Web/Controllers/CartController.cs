using FoodDelivery.BLL.Interfaces;
using FoodDelivery.BLL.Models;
using FoodDelivery.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Web.Controllers;

public sealed class CartController(ICartService cartService, ISessionCartStore cartStore) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
    {
        var carts = new List<RestaurantCartDto>();
        foreach (var (restaurantId, quantities) in cartStore.GetAll())
        {
            var result = await cartService.GetRestaurantCartAsync(restaurantId, quantities, cancellationToken);
            if (result.Succeeded && result.Value is { ItemCount: > 0 } cart)
            {
                carts.Add(cart);
            }
            else if (!result.Succeeded)
            {
                cartStore.RemoveRestaurant(restaurantId);
            }
        }

        return PartialView("_CartContents", carts.OrderBy(cart => cart.RestaurantName).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int restaurantId, int dishId, CancellationToken cancellationToken)
    {
        var quantities = cartStore.GetRestaurant(restaurantId);
        var oldQuantity = quantities.GetValueOrDefault(dishId);
        if (oldQuantity >= 99)
        {
            return BadRequest(new { success = false, message = "В корзине уже максимальное количество — 99 порций." });
        }

        quantities[dishId] = oldQuantity + 1;
        var result = await cartService.GetRestaurantCartAsync(restaurantId, quantities, cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { success = false, message = result.Errors.FirstOrDefault() ?? "Не удалось добавить блюдо." });
        }

        cartStore.SetRestaurant(restaurantId, quantities);
        return Ok(new { success = true, message = "Блюдо добавлено в корзину.", itemCount = result.Value!.ItemCount });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetQuantity(int restaurantId, int dishId, int quantity, CancellationToken cancellationToken)
    {
        if (quantity is < 1 or > 99)
        {
            return BadRequest(new { success = false, message = "Количество должно быть от 1 до 99 порций." });
        }

        var quantities = cartStore.GetRestaurant(restaurantId);
        if (!quantities.ContainsKey(dishId))
        {
            return BadRequest(new { success = false, message = "Блюдо не найдено в корзине." });
        }

        quantities[dishId] = quantity;
        var result = await cartService.GetRestaurantCartAsync(restaurantId, quantities, cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { success = false, message = result.Errors.FirstOrDefault() ?? "Не удалось обновить корзину." });
        }

        cartStore.SetRestaurant(restaurantId, quantities);
        return Ok(new { success = true, message = "Количество обновлено.", itemCount = result.Value!.ItemCount });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int restaurantId, int dishId)
    {
        var quantities = cartStore.GetRestaurant(restaurantId);
        if (!quantities.Remove(dishId))
        {
            return BadRequest(new { success = false, message = "Блюдо не найдено в корзине." });
        }

        cartStore.SetRestaurant(restaurantId, quantities);
        return Ok(new { success = true, message = "Блюдо удалено из корзины." });
    }
}
