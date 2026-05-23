using EduGestor.Models.ViewModels;
using EduGestor.Services;
using EduGestor.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EduGestor.Controllers
{
    [Authorize(Roles = "Admin,Secretary")]
    public class RegistrationsController : Controller
    {
        private readonly RegistrationService _registrationService;
        private readonly StudentService _studentService;
        private readonly StudentClassService _studentClassService;

        public RegistrationsController(
            RegistrationService registrationService,
            StudentService studentService,
            StudentClassService studentClassService
        )
        {
            _registrationService = registrationService;
            _studentService = studentService;
            _studentClassService = studentClassService;
        }

        public async Task<IActionResult> Index(RegistrationSearchViewModel filters)
        {
            var vm = await _registrationService.FindAllSearchAsync(filters);

            return View(vm);
        }

        // =========================
        // CREATE
        // =========================
        public async Task<IActionResult> Create()
        {
            var students = await _studentService.FindAllAsync();
            var sc = await _studentClassService.FindAllAsync();
            var viewModel = new RegistrationFormViewModel { Students = students, StudentClasses = sc };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistrationFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Students =
                    await _studentService.FindAllAsync();

                viewModel.StudentClasses =
                    await _studentClassService.FindAllAsync();

                return View(viewModel);
            }

            try
            {
                await _registrationService
                    .InsertAsync(viewModel.Registration);

                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException err)
            {
                ModelState.AddModelError(string.Empty, err.Message);

                viewModel.Students =
                    await _studentService.FindAllAsync();

                viewModel.StudentClasses =
                    await _studentClassService.FindAllAsync();

                return View(viewModel);
            }
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(Guid id)
        {
            var reg = await _registrationService.FindByIdAsync(id);

            if (reg == null)
            {
                throw new NotFoundException("Registration Id not found");
            }

            return View(reg);
        }

        // =========================
        // EDIT
        // =========================
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                throw new NotFoundException("Registration Id not found");
            }

            var reg = await _registrationService.FindByIdAsync(id.Value);

            if (reg == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Registration not found!" });
            }

            var students = await _studentService.FindAllAsync();
            var sc = await _studentClassService.FindAllAsync();
            var viewModel = new RegistrationFormViewModel
            {
                Registration = reg,
                Students = students,
                StudentClasses = sc
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RegistrationFormViewModel viewModel)
        {
            if (id != viewModel.Registration?.Id)
            {
                throw new NotFoundException(
                    "Registration Id not found.");
            }

            if (!ModelState.IsValid)
            {
                viewModel.Students =
                    await _studentService.FindAllAsync();

                viewModel.StudentClasses =
                    await _studentClassService.FindAllAsync();

                return View(viewModel);
            }

            try
            {
                await _registrationService
                    .UpdateAsync(viewModel.Registration);

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
            var reg = await _registrationService.FindByIdAsync(id);

            if (reg == null)
            {
                throw new NotFoundException("Registration Id not found.");
            }

            return View(reg);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _registrationService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException err)
            {
                return RedirectToAction(nameof(Error), new { message = err.Message });
            }
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
