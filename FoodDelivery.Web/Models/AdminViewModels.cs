using System.ComponentModel.DataAnnotations;
using FoodDelivery.BLL;

namespace FoodDelivery.Web.Models;

public class UserCreateViewModel
{
    [Required(ErrorMessage = "Укажите имя")]
    [StringLength(200)]
    [Display(Name = "Имя")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите пароль")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать не менее 6 символов")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Роль")]
    public string Role { get; set; } = RoleNames.User;
}

public class RoleSelectionViewModel
{
    public string Name { get; set; } = string.Empty;
    public bool Selected { get; set; }
}

public class UserRolesViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<RoleSelectionViewModel> Roles { get; set; } = new();
}
