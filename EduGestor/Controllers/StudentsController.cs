using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.Services;
using EduGestor.Services.Exceptions;
using EduGestor.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace EduGestor.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentService _studentService;
        private readonly GuardianService _guardianService;

        public StudentsController(StudentService studentService, GuardianService guardianService)
        {
            _studentService = studentService;
            _guardianService = guardianService;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index(StudentSearchViewModel filters)
        {
            ViewData["CurrentFilter"] = filters.SearchTerm;

            var vm =
                await _studentService.FindAllSearchAsync(filters);

            return View(vm);
        }

        // =========================
        // DETAILS
        // =========================

        public async Task<IActionResult> Details(Guid id)
        {
            var student = await _studentService.FindByIdAsync(id);

            if (student == null)
            {
                throw new NotFoundException("Id not found");
            }

            return View(student);
        }

        // =========================
        // CREATE
        // =========================

        public async Task<IActionResult> Create()
        {
            var guardians = await _guardianService.FindAllAsync();

            var vm = new StudentFormViewModel
            {
                Guardians = guardians.Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = $"{g.Name} ({g.Email})"
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentFormViewModel vm, IFormFile? photo)
        {
            if (!ModelState.IsValid)
            {
                vm.Guardians = (await _guardianService.FindAllAsync())
                    .Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = $"{g.Name} ({g.Email})"
                    }).ToList();

                return View(vm);
            }

            if (vm.Student == null)
            {
                return View(vm);
            }

            // FOTO
            if (photo != null && photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/students");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName =
                    Guid.NewGuid()
                    + Path.GetExtension(photo.FileName);

                var filePath =
                    Path.Combine(uploadsFolder, fileName);

                using var stream =
                    new FileStream(filePath, FileMode.Create);

                await photo.CopyToAsync(stream);

                vm.Student.FotoUrl =
                    "/uploads/students/" + fileName;
            }

            // GUARDIAN
            if (vm.CreateNewGuardian)
            {
                var existingGuardian =
                    await _guardianService
                        .FindByCpfAsync(vm.NewGuardian.Cpf);

                if (existingGuardian != null)
                {
                    vm.Student.GuardianId =
                        existingGuardian.Id;
                }
                else
                {
                    var guardian =
                        await _guardianService
                            .InsertAsync(vm.NewGuardian);

                    vm.Student.GuardianId =
                        guardian.Id;
                }
            }
            else
            {
                vm.Student.GuardianId =
                    vm.SelectedGuardianId;
            }

            try
            {
                await _studentService.InsertAsync(vm.Student);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException err)
            {
                ModelState.AddModelError(
                    string.Empty,
                    err.Message);

                vm.Guardians =
                    (await _guardianService.FindAllAsync())
                    .Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = $"{g.Name} ({g.Email})"
                    }).ToList();

                return View(vm);
            }
        }

        // =========================
        // EDIT
        // =========================

        public async Task<IActionResult> Edit(Guid id)
        {
            var student = await _studentService.FindByIdAsync(id);

            if (student == null)
            {
                throw new NotFoundException("Id not found");
            }

            var guardians = await _guardianService.FindAllAsync();

            var vm = new StudentFormViewModel
            {
                Student = student,
                SelectedGuardianId = student.GuardianId,

                Guardians = guardians.Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = $"{g.Name} ({g.Email})"
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, StudentFormViewModel vm, IFormFile? photo)
        {
            if (id != vm.Student?.Id)
            {
                throw new NotFoundException("Id not found.");
            }

            ModelState.Remove("NewGuardian");

            if (!ModelState.IsValid)
            {
                vm.Guardians = (await _guardianService.FindAllAsync())
                    .Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = $"{g.Name} ({g.Email})"
                    }).ToList();

                return View(vm);
            }

            var student = await _studentService.FindByIdForUpdateAsync(id);

            if (student == null)
            {
                throw new NotFoundException("Student Id not found.");
            }

            student.Name = vm.Student.Name;
            student.Rg = vm.Student.Rg;
            student.Cpf = vm.Student.Cpf;
            student.BirthDate = vm.Student.BirthDate;
            student.GuardianId = vm.SelectedGuardianId;

            // FOTO
            if (photo != null && photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/students");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName =
                    Guid.NewGuid() +
                    Path.GetExtension(photo.FileName);

                var filePath =
                    Path.Combine(uploadsFolder, fileName);

                using var stream =
                    new FileStream(filePath, FileMode.Create);

                await photo.CopyToAsync(stream);

                student.FotoUrl =
                    "/uploads/students/" + fileName;
            }

            try
            {
                await _studentService.UpdateAsync(student);
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
            var student = await _studentService.FindByIdAsync(id);

            if (student == null)
            {
                throw new NotFoundException("Student Id not found.");
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _studentService.RemoveAsync(id);

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
