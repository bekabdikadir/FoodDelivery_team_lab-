using System.Security.Claims;
using FoodDelivery.BLL.Interfaces;
using FoodDelivery.BLL.Models;
using FoodDelivery.Web.Models;
using FoodDelivery.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Web.Controllers;

public sealed class OrdersController(
    ICartService cartService,
    IOrderService orderService,
    ISessionCartStore cartStore,
    IOrderEmailSender emailSender) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Checkout(int restaurantId, CancellationToken cancellationToken)
    {
        var cart = await GetCartAsync(restaurantId, cancellationToken);
        if (cart is null || cart.ItemCount == 0)
        {
            TempData["Error"] = "Сначала добавьте блюда в корзину.";
            return RedirectToAction("Details", "Restaurants", new { id = restaurantId });
        }

        return View(new CheckoutViewModel
        {
            RestaurantId = restaurantId,
            Cart = cart,
            RecipientEmail = User.Identity?.IsAuthenticated == true ? User.Identity.Name ?? string.Empty : string.Empty,
            RecipientName = User.Identity?.IsAuthenticated == true ? User.Identity.Name ?? string.Empty : string.Empty
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model, CancellationToken cancellationToken)
    {
        model.Cart = await GetCartAsync(model.RestaurantId, cancellationToken);
        if (model.Cart is null || model.Cart.ItemCount == 0)
        {
            TempData["Error"] = "Корзина пуста. Добавьте блюда перед оформлением заказа.";
            return RedirectToAction("Details", "Restaurants", new { id = model.RestaurantId });
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var quantities = cartStore.GetRestaurant(model.RestaurantId);
        var result = await orderService.PlaceAsync(
            model.RestaurantId,
            quantities,
            new CreateOrderDto
            {
                RecipientName = model.RecipientName,
                RecipientEmail = model.RecipientEmail,
                RecipientPhone = model.RecipientPhone,
                DeliveryAddress = model.DeliveryAddress
            },
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            cancellationToken);

        if (!result.Succeeded || result.Value is not int orderId)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        cartStore.RemoveRestaurant(model.RestaurantId);
        var order = await orderService.GetByIdAsync(orderId, cancellationToken);
        var emailSent = order is not null && await emailSender.SendOrderConfirmationAsync(order, cancellationToken);
        TempData["OrderEmailSent"] = emailSent;
        return RedirectToAction(nameof(Confirmation), new { id = orderId });
    }

    [HttpGet]
    public IActionResult Confirmation(int id)
    {
        ViewData["OrderId"] = id;
        ViewData["EmailSent"] = TempData["OrderEmailSent"] is bool emailSent && emailSent;
        return View();
    }

    private async Task<RestaurantCartDto?> GetCartAsync(int restaurantId, CancellationToken cancellationToken)
    {
        var result = await cartService.GetRestaurantCartAsync(
            restaurantId,
            cartStore.GetRestaurant(restaurantId),
            cancellationToken);
        return result.Succeeded ? result.Value : null;
    }
}
