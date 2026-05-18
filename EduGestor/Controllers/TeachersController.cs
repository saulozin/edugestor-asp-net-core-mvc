using EduGestor.Models;
using EduGestor.Services;
using EduGestor.ViewModels;
using Microsoft.AspNetCore.Mvc;
using EduGestor.Services.Exceptions;

namespace EduGestor.Controllers
{
    public class TeachersController : Controller
    {
        private readonly TeacherService _teacherService;

        public TeachersController(TeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _teacherService.FindAllAsync();
            return View(list);
        }

        // =========================
        // CREATE
        // =========================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            await _teacherService.InsertAsync(teacher);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(Guid id)
        {
            var teacher = await _teacherService.FindByIdAsync(id);

            if(teacher == null)
            {
                throw new NotFoundException("Id not found");
            }

            return View(teacher);
        }

        // =========================
        // EDIT
        // =========================
        public async Task<IActionResult> Edit(Guid id)
        {
            var teacher = await _teacherService.FindByIdAsync(id);

            if (teacher == null)
            {
                throw new NotFoundException("Id not found");
            }

            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Teacher teacher)
        {
            if (id != teacher.Id)
            {
                throw new NotFoundException("Id not found.");
            }

            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            await _teacherService.UpdateAsync(teacher);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(Guid id)
        {
            var teacher = await _teacherService.FindByIdAsync(id);

            if (teacher == null)
            {
                throw new NotFoundException("Tearcher's Id not found.");
            }

            return View(teacher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _teacherService.RemoveAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
