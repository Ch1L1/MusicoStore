using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
    ICustomerService customerService,
    IEmailSender emailSender)
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
            PhoneNumber = model.PhoneNumber,
            Employee = model.Employee
        },
            CancellationToken.None);

        var user = new LocalIdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            CustomerId = createdCustomerDto.Id
        };

        IdentityResult result = await userManager.CreateAsync(user, model.Password);

        if (model.Employee)
        {
            await userManager.AddToRoleAsync(user, "Employee");
        }

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction(nameof(LoginSuccess), nameof(AccountController).Replace("Controller", ""));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        
        await userManager.DeleteAsync(user);
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
    
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        LocalIdentityUser? user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
            
        var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);

        await emailSender.SendEmailAsync(model.Email, "Reset Password",
            $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

        return RedirectToAction(nameof(ForgotPasswordConfirmation));

    }

    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string? token = null, string? email = null)
    {
        if (token == null || email == null)
        {
            return BadRequest("A code must be supplied for password reset.");
        }

        var model = new ResetPasswordViewModel
        {
            Token = token,
            Email = email
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        LocalIdentityUser? user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        IdentityResult result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }

    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }
}
