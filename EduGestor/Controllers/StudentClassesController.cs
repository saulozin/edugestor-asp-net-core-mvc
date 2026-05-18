using EduGestor.Models;
using EduGestor.Services;
using EduGestor.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EduGestor.Controllers
{
    public class StudentClassesController : Controller
    {
        private readonly StudentClassService _studentClassService;

        public StudentClassesController(StudentClassService studentClassService)
        {
            _studentClassService = studentClassService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _studentClassService.FindAllAsync();
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
        public async Task<IActionResult> Create(StudentClass obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            await _studentClassService.InsertAsync(obj);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT
        // =========================
        public async Task<IActionResult> Edit(Guid id)
        {
            var sClass = await _studentClassService.FindByIdAsync(id);

            if (sClass == null)
            {
                throw new NotFoundException("Student Class Id not found");
            }

            return View(sClass);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, StudentClass sClass)
        {
            if (id != sClass.Id)
            {
                throw new NotFoundException("Student Class Id not found.");
            }

            if (!ModelState.IsValid)
            {
                return View(sClass);
            }

            await _studentClassService.UpdateAsync(sClass);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(Guid id)
        {
            var sClass = await _studentClassService.FindByIdAsync(id);

            if (sClass == null)
            {
                throw new NotFoundException("Student Class Id not found");
            }

            return View(sClass);
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(Guid id)
        {
            var sClass = await _studentClassService.FindByIdAsync(id);

            if (sClass == null)
            {
                throw new NotFoundException("Student Class Id not found.");
            }

            return View(sClass);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _studentClassService.RemoveAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
