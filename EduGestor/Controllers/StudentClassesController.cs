using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.Services;
using EduGestor.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EduGestor.Controllers
{
    [Authorize(Roles = "Admin,Secretary")]
    public class StudentClassesController : Controller
    {
        private readonly StudentClassService _studentClassService;

        public StudentClassesController(StudentClassService studentClassService)
        {
            _studentClassService = studentClassService;
        }

        public async Task<IActionResult> Index(StudentClassSearchViewModel filters)
        {
            var vm = await _studentClassService.FindAllSearchAsync(filters);

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
        public async Task<IActionResult> Create(StudentClass obj)
        {
            if (obj == null)
            {
                return View(obj);
            }

            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            try
            {
                await _studentClassService.InsertAsync(obj);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException err)
            {
                ModelState.AddModelError(
                    string.Empty,
                    err.Message);

                return View(obj);
            }
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

            try
            {
                await _studentClassService.UpdateAsync(sClass);
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
            catch (IntegrityException err)
            {
                ModelState.AddModelError(string.Empty, err.Message);

                return View(sClass);
            }
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
