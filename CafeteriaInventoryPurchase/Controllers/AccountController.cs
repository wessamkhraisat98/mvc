using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeteriaInventoryPurchase.Controllers;

[AutoValidateAntiforgeryToken]
public class AccountController(IConfiguration config) : Controller
{
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string? password)
    {
        string? expected = config["EmployeePassword"];
        if (string.IsNullOrWhiteSpace(expected) || password != expected)
        {
            ModelState.AddModelError("", "Invalid employee password or sign-in is not configured.");
            return View();
        }

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, "Cafeteria employee") },
            CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(new ClaimsPrincipal(identity));
        return RedirectToAction("Index", "Products");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
}
