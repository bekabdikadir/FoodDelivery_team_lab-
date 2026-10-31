using System.ComponentModel.DataAnnotations;
using FoodDelivery.BLL.Models;

namespace FoodDelivery.Web.Models;

public sealed class CheckoutViewModel
{
    public int RestaurantId { get; set; }
    public RestaurantCartDto? Cart { get; set; }

    [Required(ErrorMessage = "Укажите имя получателя.")]
    [StringLength(200, MinimumLength = 2)]
    [Display(Name = "Имя получателя")]
    public string RecipientName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите электронную почту.")]
    [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты.")]
    [StringLength(256)]
    [Display(Name = "Электронная почта для письма с заказом")]
    public string RecipientEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите номер телефона.")]
    [Phone(ErrorMessage = "Введите корректный номер телефона.")]
    [StringLength(40, MinimumLength = 5)]
    [Display(Name = "Телефон")]
    public string RecipientPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите адрес доставки.")]
    [StringLength(1000, MinimumLength = 5)]
    [Display(Name = "Адрес доставки")]
    public string DeliveryAddress { get; set; } = string.Empty;
}
