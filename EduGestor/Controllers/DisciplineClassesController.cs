using Microsoft.AspNetCore.Mvc;
using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.Services;
using EduGestor.Services.Exceptions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace EduGestor.Controllers
{
    public class DisciplineClassesController : Controller
    {
        private readonly DisciplineClassService _disciplineClassService;
        private readonly StudentClassService _studentClassService;
        private readonly DisciplineService _disciplineService;
        private readonly TeacherService _teacherService;

        public DisciplineClassesController(
            DisciplineClassService disciplineClassService,
            StudentClassService studentClassService,
            DisciplineService disciplineService,
            TeacherService teacherService)
        {
            _disciplineClassService = disciplineClassService;
            _studentClassService = studentClassService;
            _disciplineService = disciplineService;
            _teacherService = teacherService;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var list = await _disciplineClassService.FindAllAsync();

            return View(list);
        }

        // =========================
        // CREATE
        // =========================
        public async Task<IActionResult> Create()
        {
            var vm = await BuildViewModel();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            DisciplineClassFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await BuildViewModel(vm.DisciplineClass);

                return View(vm);
            }

            try
            {
                await _disciplineClassService
                    .InsertAsync(vm.DisciplineClass!);

                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException err)
            {
                ModelState.AddModelError(
                    string.Empty,
                    err.Message);

                vm = await BuildViewModel(vm.DisciplineClass);

                return View(vm);
            }
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(Guid id)
        {
            var dc = await _disciplineClassService
                .FindByIdAsync(id);

            if (dc == null)
            {
                throw new NotFoundException(
                    "DisciplineClass Id not found.");
            }

            return View(dc);
        }

        // =========================
        // EDIT
        // =========================
        public async Task<IActionResult> Edit(Guid id)
        {
            var dc = await _disciplineClassService
                .FindByIdAsync(id);

            if (dc == null)
            {
                throw new NotFoundException(
                    "DisciplineClass Id not found.");
            }

            var vm = await BuildViewModel(dc);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Guid id,
            DisciplineClassFormViewModel vm)
        {
            if (id != vm.DisciplineClass?.Id)
            {
                throw new NotFoundException(
                    "DisciplineClass Id not found.");
            }

            if (!ModelState.IsValid)
            {
                vm = await BuildViewModel(vm.DisciplineClass);

                return View(vm);
            }

            try
            {
                await _disciplineClassService
                    .UpdateAsync(vm.DisciplineClass!);

                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException err)
            {
                ModelState.AddModelError(
                    string.Empty,
                    err.Message);

                vm = await BuildViewModel(vm.DisciplineClass);

                return View(vm);
            }
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(Guid id)
        {
            var dc = await _disciplineClassService
                .FindByIdAsync(id);

            if (dc == null)
            {
                throw new NotFoundException(
                    "DisciplineClass Id not found.");
            }

            return View(dc);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _disciplineClassService
                    .RemoveAsync(id);

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
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            };

            return View(viewModel);
        }

        // =========================
        // BUILD VIEWMODEL
        // =========================
        private async Task<DisciplineClassFormViewModel>BuildViewModel(DisciplineClass? dc = null)
        {
            return new DisciplineClassFormViewModel
            {
                DisciplineClass = dc ?? new DisciplineClass(),

                StudentClasses =
                    (await _studentClassService.FindAllAsync())
                    .Select(sc => new SelectListItem
                    {
                        Value = sc.Id.ToString(),
                        Text = sc.Code
                    }).ToList(),

                Disciplines =
                    (await _disciplineService.FindAllAsync())
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    }).ToList(),

                Teachers =
                    (await _teacherService.FindAllAsync())
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Name
                    }).ToList()
            };
        }
    }
}
