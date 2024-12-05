using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserManagementApp.Models;

public class RegisterController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    //public IActionResult Register()
    //{
    //    return View();
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Optionally sign in the user
                return RedirectToAction("Index", "Home");
            }
            else {
                foreach (var error in result.Errors)
                {
                    switch (error.Code)
                    {
                        case "DuplicateUserName":
                            ModelState.AddModelError("Username", "Username is already taken. Please choose a different one.");
                            break;
                        case "DuplicateEmail":
                            ModelState.AddModelError("Email", "An account with this email already exists.");
                            break;
                        case "PasswordTooShort":
                            ModelState.AddModelError("Password", "Password must be at least 6 characters long.");
                            break;
                        case "InvalidEmail":
                            ModelState.AddModelError("Email", "Email address is invalid.");
                            break;
                        // Add more cases as needed
                        default:
                            ModelState.AddModelError(string.Empty, "An unknown error occurred. Please try again.");
                            break;
                    }
                }
            }
        }
        return View();
    }
}
