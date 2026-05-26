using EduGestor.Models.Identity;
using EduGestor.Models.ViewModels;
using EduGestor.Services;
using EduGestor.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EduGestor.Controllers
{
    [Authorize(Roles = "Guardian")]
    public class PortalController : Controller
    {
        private readonly PortalService _portalService;

        private readonly UserManager<AppUser> _userManager;

        public PortalController(
            PortalService portalService,
            UserManager<AppUser> userManager)
        {
            _portalService = portalService;

            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                var vm = await _portalService
                    .GetPortalDataAsync(user.Email!);

                return View(vm);
            }
            catch (IntegrityException err)
            {
                return RedirectToAction(nameof(Error), new { message = err.Message });
            }
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(viewModel);
        }
    }
}
