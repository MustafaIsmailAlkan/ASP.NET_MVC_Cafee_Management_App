using Cafee_Prototype.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cafee_Prototype.Controllers;

public class AccountController : Controller
{
    private UserManager<IdentityUser> _userManager;
    private SignInManager<IdentityUser> _signInManager;
    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest("Account Login ModelState is not valid");
        }

        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user == null)
        {
            return BadRequest("Account Login user is null");
        }

        await _signInManager.SignOutAsync();
        var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);

        if (result.Succeeded)
        {
            await _userManager.ResetAccessFailedCountAsync(user);
            await _userManager.SetLockoutEndDateAsync(user, null);

            return RedirectToAction("Main", "Home");
        }
        else if(result.IsLockedOut)
        {
            var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
            var timeLeft = lockoutDate.Value - DateTime.UtcNow;
            ModelState.AddModelError("", "Your account is locked, try again " + lockoutDate + " minutes later.");
        }
        else
        {
            ModelState.AddModelError("", "Wrong Password or Username");
        }

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Main", "Home");
    }
}