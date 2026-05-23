using EduGestor.Models.Identity;
using EduGestor.Models.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EduGestor.Data;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly EduGestorContext _context;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            EduGestorContext context)
        {
            _userManager = userManager;

            _signInManager = signInManager;

            _context = context;
        }

        // =========================
        // REGISTER
        // =========================

        [Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
                // ADD ROLE
                await _userManager.AddToRoleAsync(
                    user,
                    model.Role);

                // LINK GUARDIAN
                if (model.Role == "Guardian")
                {
                    var guardian =
                        await _context.Guardians
                            .FirstOrDefaultAsync(g =>
                                g.Email == model.Email);

                    if (guardian != null)
                    {
                        guardian.UserId = user.Id;

                        await _context.SaveChangesAsync();
                    }
                }

                TempData["Success"] = "User created successfully.";

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
                var user =
                    await _userManager.FindByEmailAsync(model.Email);

                // GUARDIAN
                if (user != null &&
                    await _userManager.IsInRoleAsync(
                        user,
                        "Guardian"))
                {
                    return RedirectToAction(
                        "Index",
                        "Portal");
                }

                // ADMIN AREA
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
