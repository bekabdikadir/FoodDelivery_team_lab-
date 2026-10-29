using FoodDelivery.BLL.Interfaces;
using FoodDelivery.BLL.Models;
using FoodDelivery.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.BLL.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await userManager.Users.AsNoTracking().OrderBy(u => u.Email).ToListAsync();
        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(ToDto(user, roles));
        }

        return result;
    }

    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        return ToDto(user, roles);
    }

    public async Task<ServiceResult> CreateAsync(CreateUserDto dto)
    {
        var role = RoleNames.All.Contains(dto.Role) ? dto.Role : RoleNames.User;
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

        var roleResult = await userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            return ServiceResult.Failure(roleResult.Errors.Select(e => e.Description).ToArray());
        }

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(string id, string? currentUserId)
    {
        if (id == currentUserId)
        {
            return ServiceResult.Failure("Нельзя удалить самого себя");
        }

        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult.Failure("Пользователь не найден");
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return ServiceResult.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SetRolesAsync(string id, IEnumerable<string> roles, string? currentUserId)
    {
        if (id == currentUserId)
        {
            return ServiceResult.Failure("Нельзя изменять собственные роли");
        }

        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult.Failure("Пользователь не найден");
        }

        var newRoles = roles.Where(r => RoleNames.All.Contains(r)).Distinct().ToList();
        if (newRoles.Count == 0)
        {
            return ServiceResult.Failure("Выберите хотя бы одну роль");
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        var toRemove = currentRoles.Except(newRoles).ToList();
        var toAdd = newRoles.Except(currentRoles).ToList();

        if (toRemove.Count > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, toRemove);
            if (!removeResult.Succeeded)
            {
                return ServiceResult.Failure(removeResult.Errors.Select(e => e.Description).ToArray());
            }
        }

        if (toAdd.Count > 0)
        {
            var addResult = await userManager.AddToRolesAsync(user, toAdd);
            if (!addResult.Succeeded)
            {
                return ServiceResult.Failure(addResult.Errors.Select(e => e.Description).ToArray());
            }
        }

        return ServiceResult.Success();
    }

    private static UserDto ToDto(ApplicationUser user, IEnumerable<string> roles)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Roles = roles.OrderBy(r => r).ToList()
        };
    }
}
