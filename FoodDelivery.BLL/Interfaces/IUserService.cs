using FoodDelivery.BLL.Models;

namespace FoodDelivery.BLL.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(string id);
    Task<ServiceResult> CreateAsync(CreateUserDto dto);
    Task<ServiceResult> DeleteAsync(string id, string? currentUserId);
    Task<ServiceResult> SetRolesAsync(string id, IEnumerable<string> roles, string? currentUserId);
}
