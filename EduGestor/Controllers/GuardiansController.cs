using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.Services;
using EduGestor.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EduGestor.Controllers
{
    public class GuardiansController : Controller
    {
        private readonly GuardianService _guardianService;

        public GuardiansController(GuardianService guardianService)
        {
            _guardianService = guardianService;
        }

        // =========================
        // INDEX
        // =========================

        public async Task<IActionResult> Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var guardians = await _guardianService.FindAllSearchAsync(searchString);

            return View(guardians);
        }

        // =========================
        // DETAILS
        // =========================

        public async Task<IActionResult> Details(Guid id)
        {
            var guardian = await _guardianService.FindByIdAsync(id);

            if (guardian == null)
            {
                throw new NotFoundException("Id not found.");
            }

            return View(guardian);
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
        public async Task<IActionResult> Create(Guardian guardian)
        {
            if (guardian == null)
            {
                return View(guardian);
            }

            if (!ModelState.IsValid)
            {
                return View(guardian);
            }

            try
            {
                await _guardianService.InsertAsync(guardian);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException err)
            {
                ModelState.AddModelError(
                    string.Empty,
                    err.Message);

                return View(guardian);
            }
        }

        // =========================
        // EDIT
        // =========================

        public async Task<IActionResult> Edit(Guid id)
        {
            var guardian =
                await _guardianService.FindByIdAsync(id);

            if (guardian == null)
            {
                throw new NotFoundException("Id not found");
            }

            return View(guardian);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Guid id,
            Guardian guardian)
        {
            if (id != guardian.Id)
            {
                throw new NotFoundException("Id not found");
            }

            if (!ModelState.IsValid)
            {
                return View(guardian);
            }

            try
            {
                await _guardianService.UpdateAsync(guardian);
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
            var guardian =
                await _guardianService.FindByIdAsync(id);

            if (guardian == null)
            {
                throw new NotFoundException("Id not found.");
            }

            return View(guardian);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _guardianService.RemoveAsync(id);
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
