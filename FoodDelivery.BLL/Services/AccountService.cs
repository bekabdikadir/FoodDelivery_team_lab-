using FoodDelivery.BLL.Interfaces;
using FoodDelivery.BLL.Models;
using FoodDelivery.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace FoodDelivery.BLL.Services;

public class AccountService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : IAccountService
{
    public async Task<ServiceResult> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return ServiceResult.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        await userManager.AddToRoleAsync(user, RoleNames.User);
        await signInManager.SignInAsync(user, false);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> LoginAsync(string email, string password, bool rememberMe)
    {
        var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, true);
        if (result.Succeeded)
        {
            return ServiceResult.Success();
        }

        if (result.IsLockedOut)
        {
            return ServiceResult.Failure("Аккаунт временно заблокирован из-за множества неудачных попыток входа");
        }

        return ServiceResult.Failure("Неверный email или пароль");
    }

    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }
}
