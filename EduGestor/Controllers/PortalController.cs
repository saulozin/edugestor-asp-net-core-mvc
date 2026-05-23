using EduGestor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduGestor.Controllers
{
    [Authorize(Roles = "Guardian")]
    public class PortalController : Controller
    {
        private readonly PortalService _portalService;

        public PortalController(
            PortalService portalService)
        {
            _portalService = portalService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await _portalService.GetGuardianPortalAsync(User);

            return View(vm);
        }
    }
}
