using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.Services;
using EduGestor.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace EduGestor.Controllers
{
    public class GradesController : Controller
    {
        private readonly GradeService _gradeService;
        private readonly RegistrationService _registrationService;
        private readonly DisciplineClassService _disciplineClassService;

        public GradesController(
            GradeService gradeService,
            RegistrationService registrationService,
            DisciplineClassService disciplineClassService)
        {
            _gradeService = gradeService;
            _registrationService = registrationService;
            _disciplineClassService = disciplineClassService;
        }

        // =========================
        // SELECT LISTS
        // =========================

        private async Task<List<SelectListItem>>
            GetRegistrationsSelectList()
        {
            var regs =
                await _registrationService.FindAllAsync();

            return regs.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text =
                    $"{r.Student!.Name} - {r.StudentClass!.Code}"
            }).ToList();
        }

        private async Task<List<SelectListItem>>
            GetDisciplineClassesSelectList()
        {
            var disciplineClasses =
                await _disciplineClassService.FindAllAsync();

            return disciplineClasses.Select(dc => new SelectListItem
            {
                Value = dc.Id.ToString(),
                Text =
                    $"{dc.Discipline!.Name} - " +
                    $"{dc.StudentClass!.Code} - " +
                    $"{dc.Teacher!.Name}"
            }).ToList();
        }

        private async Task BuildViewModelAsync(
            GradeFormViewModel vm)
        {
            vm.Registrations =
                await GetRegistrationsSelectList();

            vm.DisciplineClasses =
                await GetDisciplineClassesSelectList();
        }

        // =========================
        // INDEX
        // =========================

        public async Task<IActionResult> Index(GradeSearchViewModel filters)
        {
            var vm = await _gradeService.FindAllSearchAsync(filters);

            return View(vm);
        }

        // =========================
        // CREATE
        // =========================

        public async Task<IActionResult> Create()
        {
            var vm = new GradeFormViewModel
            {
                Grade = new Grade()
            };

            await BuildViewModelAsync(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            GradeFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await BuildViewModelAsync(vm);

                return View(vm);
            }

            try
            {
                await _gradeService.InsertAsync(vm.Grade!);

                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException err)
            {
                ModelState.AddModelError(
                    string.Empty,
                    err.Message);

                await BuildViewModelAsync(vm);

                return View(vm);
            }
        }

        // =========================
        // DETAILS
        // =========================

        public async Task<IActionResult> Details(Guid id)
        {
            var grade =
                await _gradeService.FindByIdAsync(id);

            if (grade == null)
            {
                throw new NotFoundException(
                    "Grade Id not found.");
            }

            return View(grade);
        }

        // =========================
        // EDIT
        // =========================

        public async Task<IActionResult> Edit(Guid id)
        {
            var grade =
                await _gradeService.FindByIdAsync(id);

            if (grade == null)
            {
                throw new NotFoundException(
                    "Grade Id not found.");
            }

            var vm = new GradeFormViewModel
            {
                Grade = grade
            };

            await BuildViewModelAsync(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Guid id,
            GradeFormViewModel vm)
        {
            if (id != vm.Grade?.Id)
            {
                throw new NotFoundException(
                    "Grade Id not found.");
            }

            if (!ModelState.IsValid)
            {
                await BuildViewModelAsync(vm);

                return View(vm);
            }

            try
            {
                await _gradeService.UpdateAsync(vm.Grade);

                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException err)
            {
                ModelState.AddModelError(
                    string.Empty,
                    err.Message);

                await BuildViewModelAsync(vm);

                return View(vm);
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
            var grade =
                await _gradeService.FindByIdAsync(id);

            if (grade == null)
            {
                throw new NotFoundException(
                    "Grade Id not found.");
            }

            return View(grade);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            Guid id)
        {
            try
            {
                await _gradeService.RemoveAsync(id);

                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException err)
            {
                return RedirectToAction(
                    nameof(Error),
                    new { message = err.Message });
            }
        }

        // =========================
        // ERROR
        // =========================

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId =
                    Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            };

            return View(viewModel);
        }
    }
}
