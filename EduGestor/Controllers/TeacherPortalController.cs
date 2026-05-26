using EduGestor.Services;
using EduGestor.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduGestor.Controllers
{
    [Authorize]
    public class TeacherPortalController : Controller
    {
        private readonly TeacherPortalService _teacherPortalService;

        public TeacherPortalController(TeacherPortalService teacherPortalService)
        {
            _teacherPortalService = teacherPortalService;
        }

        public async Task<IActionResult> Index()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var isAdmin = User.IsInRole("Admin");

            var vm =
                await _teacherPortalService
                    .GetPortalDataAsync(email, isAdmin);

            if (vm == null)
            {
                throw new NotFoundException(
                    "Teacher portal data not found.");
            }

            return View(vm);
        }

        public async Task<IActionResult> Details(Guid disciplineClassId)
        {
            var vm =
                await _teacherPortalService
                    .GetClassDetailsAsync(disciplineClassId);

            if (vm == null)
            {
                throw new NotFoundException(
                    "Class not found.");
            }

            return View(vm);
        }
    }
}
