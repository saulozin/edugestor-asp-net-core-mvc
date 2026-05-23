using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.Services;
using EduGestor.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EduGestor.Controllers
{
    public class DisciplinesController : Controller
    {
        private readonly DisciplineService _disciplineService;

        public DisciplinesController(DisciplineService disciplineService)
        {
            _disciplineService = disciplineService;
        }

        // =======================
        // INDEX
        // =======================
        public async Task<IActionResult> Index(PagedViewModel<Discipline> filters)
        {

            var vm = await _disciplineService.FindAllSearchAsync(filters);

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
        public async Task<IActionResult> Create(Discipline disc)
        {
            if (disc == null)
            {
                return View(disc);
            }

            if (!ModelState.IsValid)
            {
                return View(disc);
            }

            try
            {
                await _disciplineService.InsertAsync(disc);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException err)
            {
                ModelState.AddModelError(
                    string.Empty,
                    err.Message);

                return View(disc);
            }
        }

        // =========================
        // EDIT
        // =========================
        public async Task<IActionResult> Edit(Guid id)
        {
            var disc = await _disciplineService.FindByIdAsync(id);

            if (disc == null)
            {
                throw new NotFoundException("Discipline Id not found");
            }

            return View(disc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Discipline disc)
        {
            if (id != disc.Id)
            {
                throw new NotFoundException("Discipline Id not found.");
            }

            if (!ModelState.IsValid)
            {
                return View(disc);
            }

            try
            {
                await _disciplineService.UpdateAsync(disc);
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

                return View(disc);
            }
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(Guid id)
        {
            var disc = await _disciplineService.FindByIdAsync(id);

            if (disc == null)
            {
                throw new NotFoundException("Discipline Id not found");
            }

            return View(disc);
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(Guid id)
        {
            var disc = await _disciplineService.FindByIdAsync(id);

            if (disc == null)
            {
                throw new NotFoundException("Discipline Id not found.");
            }

            return View(disc);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _disciplineService.RemoveAsync(id);
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
