using EduGestor.Models.Identity;
using EduGestor.Models.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduGestor.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;

            _signInManager = signInManager;
        }

        // =========================
        // REGISTER
        // =========================

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new AppUser
            {
                FullName = model.FullName,
                UserName = model.Email,
                Email = model.Email
            };

            var validRoles = new[]
            {
                "Secretary",
                "Teacher",
                "Guardian"
            };

            if (!validRoles.Contains(model.Role))
            {
                ModelState.AddModelError("", "Invalid role.");

                return View(model);
            }

            var result = await _userManager
                .CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                await _signInManager.SignInAsync(
                    user,
                    isPersistent: false);

                return RedirectToAction(
                    "Index",
                    "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return View(model);
        }

        // =========================
        // LOGIN
        // =========================

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result =
                await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    false);

            if (result.Succeeded)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            ModelState.AddModelError(
                string.Empty,
                "Invalid login attempt.");

            return View(model);
        }

        // =========================
        // LOGOUT
        // =========================

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(
                "Login",
                "Account");
        }

        // =========================
        // ACCESS DENIED
        // =========================

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
