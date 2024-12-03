using Cafee_Prototype.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cafee_Prototype.Controllers;

[Authorize]
public class UsersController : Controller
{
    private UserManager<IdentityUser> _userManager;
    public UsersController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult ListUsers()
    {
        return View(_userManager.Users);
    }

    public IActionResult AddUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(AddUserViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest("There is something wrong with the ModelState");
        }
        if(model.UserName == "Admin")
        {
            return BadRequest("There can only be one user named Admin");
        }

        var user = new IdentityUser { UserName = model.UserName };

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);

        if(result.Succeeded)
        {
            return RedirectToAction("ListUsers");
        }

        foreach (IdentityError err in result.Errors)
        {
            ModelState.AddModelError("", err.Description);
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return BadRequest("user is null");
        }

        await _userManager.DeleteAsync(user);

        return RedirectToAction("ListUsers");
    }

}