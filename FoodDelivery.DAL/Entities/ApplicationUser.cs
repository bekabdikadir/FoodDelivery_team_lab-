using Microsoft.AspNetCore.Identity;

namespace FoodDelivery.DAL.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
