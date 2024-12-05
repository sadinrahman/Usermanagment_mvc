using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagementApp.Models;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public const string SessionKeyName = "username";
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<HomeController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
        
    }

    public IActionResult Login()
    {
        if (HttpContext.Session.GetString(SessionKeyName) != null)
        {
            return RedirectToAction("Userinterface");
        }
        else
        {

        return View();
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (model.UserName == "admin" && model.Password == "Admin@123")
        {
            HttpContext.Session.SetString(SessionKeyName, model.UserName);
            return Redirect("/Home/Admin");
        }

        
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                HttpContext.Session.SetString(SessionKeyName, model.UserName);
                return RedirectToAction("Userinterface", "Home");

            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
        return View(model);
    }

 
    public IActionResult Register()
    {
        if (HttpContext.Session.GetString(SessionKeyName) != null)
        {
            return RedirectToAction("Userinterface");
        }
        else
        {
            return View();
        }
    }

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
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Login", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

 
    public IActionResult Userinterface()
    {
        if (HttpContext.Session.GetString(SessionKeyName) == null)
        {
            return RedirectToAction("Login");
        }
        else
        {
            return View();
        }
    }           


    public IActionResult Admin()
    {
        if (HttpContext.Session.GetString(SessionKeyName) == null)
        {
            return RedirectToAction("Login");
        }
        else
        {
            return View();
        }
    }

  
    public IActionResult signout()
    {
        HttpContext.Session.Clear(); 
        _signInManager.SignOutAsync(); 
        return RedirectToAction("Login", "Home"); 
    }
    public IActionResult Usermanager(string searchString)
    {
        if (HttpContext.Session.GetString(SessionKeyName) == "admin")
        {
            var users = _userManager.Users.ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u =>
                    u.UserName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
            return View(users); 
        }
        else
        {
            return RedirectToAction("Login");
        }
    }
    [HttpGet]
    public async Task<IActionResult> EditUser(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(string id, IdentityUser model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.UserName = model.UserName;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Usermanager");
            }
        }

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction("Usermanager");
        }

        return RedirectToAction("Usermanager");
    }
    public IActionResult Createuser()
    {
        if (HttpContext.Session.GetString(SessionKeyName) == "admin")
        {
            return View();
        }
        else
        {
            return RedirectToAction("Login");
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Createuser(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Usermanager", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }
}