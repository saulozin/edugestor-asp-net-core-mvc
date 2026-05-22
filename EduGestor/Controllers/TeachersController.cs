using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.Services;
using EduGestor.Services.Exceptions;
using EduGestor.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EduGestor.Controllers
{
    public class TeachersController : Controller
    {
        private readonly TeacherService _teacherService;

        public TeachersController(TeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        public async Task<IActionResult> Index(PagedViewModel<Teacher> filters)
        {
            ViewData["CurrentFilter"] = filters.SearchTerm;

            var vm = await _teacherService.FindAllSearchAsync(filters);
            return View(vm);
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

            if (teacher == null)
            {
                return View(teacher);
            }

            try
            {
                await _teacherService.InsertAsync(teacher);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException err)
            {
                ModelState.AddModelError(
                    string.Empty,
                    err.Message);

                return View(teacher);
            }
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

            try
            {
                await _teacherService.UpdateAsync(teacher);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException err)
            {
                return RedirectToAction(
                    nameof(Error),
                    new { message = err.Message });
            }
            catch (DbConcurrencyException err)
            {
                return RedirectToAction(
                    nameof(Error),
                    new { message = err.Message });
            }
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
