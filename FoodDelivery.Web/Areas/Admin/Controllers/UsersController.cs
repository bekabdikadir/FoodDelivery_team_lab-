using System.Security.Claims;
using FoodDelivery.BLL;
using FoodDelivery.BLL.Interfaces;
using FoodDelivery.BLL.Models;
using FoodDelivery.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class UsersController(IUserService userService) : Controller
{
    private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    public async Task<IActionResult> Index()
    {
        ViewData["CurrentUserId"] = CurrentUserId;
        return View(await userService.GetAllAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new UserCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await userService.CreateAsync(new CreateUserDto
        {
            Email = model.Email,
            FullName = model.FullName,
            Password = model.Password,
            Role = model.Role
        });

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        TempData["Success"] = "Пользователь создан";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await userService.DeleteAsync(id, CurrentUserId);
        if (result.Succeeded)
        {
            TempData["Success"] = "Пользователь удалён";
        }
        else
        {
            TempData["Error"] = string.Join(". ", result.Errors);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Roles(string id)
    {
        var user = await userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(new UserRolesViewModel
        {
            Id = user.Id,
            Email = user.Email,
            Roles = RoleNames.All.Select(r => new RoleSelectionViewModel
            {
                Name = r,
                Selected = user.Roles.Contains(r)
            }).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Roles(UserRolesViewModel model)
    {
        var selected = model.Roles.Where(r => r.Selected).Select(r => r.Name).ToList();
        var result = await userService.SetRolesAsync(model.Id, selected, CurrentUserId);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        TempData["Success"] = "Роли обновлены";
        return RedirectToAction(nameof(Index));
    }
}
