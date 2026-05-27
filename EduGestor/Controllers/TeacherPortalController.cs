using EduGestor.Models.ViewModels;
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

        // =========================================
        // DASHBOARD
        // =========================================
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

        // =========================================
        // VIEW CLASS
        // =========================================
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

        // =========================================
        // LAUNCH GRADES - GET
        // =========================================

        public async Task<IActionResult> LaunchGrades(
            Guid disciplineClassId,
            int bimester = 1)
        {
            var vm =
                await _teacherPortalService
                    .GetLaunchGradesDataAsync(
                        disciplineClassId,
                        bimester);

            if (vm == null)
            {
                throw new NotFoundException(
                    "Class not found.");
            }

            return View(vm);
        }

        // =========================================
        // LAUNCH GRADES - POST
        // =========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LaunchGrades(GradeLaunchViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _teacherPortalService.SaveLaunchGradesAsync(vm);

            return RedirectToAction(
                nameof(Details),
                new
                {
                    disciplineClassId = vm.DisciplineClassId
                });
        }

        // ===========================
        // Attendance
        // ===========================
        public async Task<IActionResult> LaunchAttendance(
            Guid disciplineClassId, DateOnly? date)
        {
            var vm =
                await _teacherPortalService
                    .GetLaunchAttendanceDataAsync(
                        disciplineClassId,
                        date ?? DateOnly.FromDateTime(DateTime.Today));

            if (vm == null)
            {
                throw new NotFoundException("Class not found.");
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LaunchAttendance(AttendanceLaunchViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _teacherPortalService.SaveLaunchAttendanceAsync(vm);

            return RedirectToAction(
                nameof(Details),
                new
                {
                    disciplineClassId = vm.DisciplineClassId
                });
        }
    }
}
