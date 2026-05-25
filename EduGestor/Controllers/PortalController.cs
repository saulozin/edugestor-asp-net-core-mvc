using EduGestor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EduGestor.Models.Identity;

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
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var vm =
                await _portalService
                    .GetPortalDataAsync(user.Email!);

            return View(vm);
        }
    }
}
