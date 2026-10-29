using FoodDelivery.BLL.Models;

namespace FoodDelivery.BLL.Interfaces;

public interface IAccountService
{
    Task<ServiceResult> RegisterAsync(RegisterDto dto);
    Task<ServiceResult> LoginAsync(string email, string password, bool rememberMe);
    Task LogoutAsync();
}
