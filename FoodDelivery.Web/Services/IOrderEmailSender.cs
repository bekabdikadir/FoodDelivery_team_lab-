using FoodDelivery.BLL.Models;

namespace FoodDelivery.Web.Services;

public interface IOrderEmailSender
{
    Task<bool> SendOrderConfirmationAsync(OrderDetailsDto order, CancellationToken cancellationToken = default);
}
