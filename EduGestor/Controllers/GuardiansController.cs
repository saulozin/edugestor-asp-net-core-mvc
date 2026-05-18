using EduGestor.Models;
using EduGestor.Services;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> Index()
        {
            var guardians = await _guardianService.FindAllAsync();

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
                return NotFound();
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
            if (!ModelState.IsValid)
            {
                return View(guardian);
            }

            var existing =
                await _guardianService.FindByCpfAsync(guardian.Cpf);

            if (existing != null)
            {
                ModelState.AddModelError(
                    "Cpf",
                    "Guardian already exists.");

                return View(guardian);
            }

            await _guardianService.InsertAsync(guardian);

            return RedirectToAction(nameof(Index));
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
                return NotFound();
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
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(guardian);
            }

            await _guardianService.UpdateAsync(guardian);

            return RedirectToAction(nameof(Index));
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
                return NotFound();
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
    }
}
