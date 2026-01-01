using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicoStore.DataAccessLayer.Identity;
using MusicoStore.Domain.DTOs.Customer;
using MusicoStore.Domain.Interfaces.Service;
using WebMVC.Models.Account;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WebMVC.Controllers;

public class AccountController(
    UserManager<LocalIdentityUser> userManager,
    SignInManager<LocalIdentityUser> signInManager,
    ICustomerService customerService)
    : Controller
{
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        CustomerDTO createdCustomerDto = await customerService.CreateAsync(new CreateCustomerDTO
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber
        },
            CancellationToken.None);

        var user = new LocalIdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            CustomerId = createdCustomerDto.Id
        };

        IdentityResult result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction(nameof(Login), nameof(AccountController).Replace("Controller", ""));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        await customerService.DeleteByIdAsync(user.CustomerId, CancellationToken.None);
        return View(model);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        SignInResult result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(LoginSuccess), nameof(AccountController).Replace("Controller", ""));
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
    }

    public IActionResult LoginSuccess()
    {
        return View();
    }
}
