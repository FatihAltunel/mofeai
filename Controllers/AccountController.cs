using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieGPT.Models;

public class AccountController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // GET: /Account/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    
    public async Task<IActionResult> Login(LoginViewModel model)
{
    if (ModelState.IsValid)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            ViewData["ErrorMessage"] = "Invalid credentials";  // Bu satırı ekledik.
            return View(model);
        }
        else
        {
            ModelState.AddModelError(string.Empty, "User not found.");
            ViewData["ErrorMessage"] = "Invalid credentials";  // Burada da aynı şekilde hata mesajını atıyoruz.
            return View(model);
        }
    }
    ViewData["ErrorMessage"] = "Invalid credentials"; // Varsayılan hata mesajı.
    return View(model);
}

    // GET: /Account/Register
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User { UserName = model.UserName, Email = model.Email };
            bool PasswordControl = model.Password == model.ConfirmPassword ? true : false;
            var result = await _userManager.CreateAsync(user, model.Password);

            if(!PasswordControl){
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
            }


            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
