using EduGestor.Data;
using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class TeacherPortalService
    {
        private readonly EduGestorContext _context;

        public TeacherPortalService(
            EduGestorContext context)
        {
            _context = context;
        }

        public async Task<TeacherPortalViewModel?>
            GetPortalDataAsync(
                string email,
                bool isAdmin = false)
        {
            var vm =
                new TeacherPortalViewModel();

            // =========================================
            // ADMIN -> VISUALIZA TODAS AS TURMAS
            // =========================================

            if (isAdmin)
            {
                vm.TeacherName = "Administrator";

                var allDisciplineClasses =
                    await _context.DisciplineClasses
                        .Include(dc => dc.Discipline)
                        .Include(dc => dc.StudentClass)
                        .Include(dc => dc.Teacher)
                        .Include(dc => dc.Grades)
                        .ToListAsync();

                foreach (var disciplineClass in allDisciplineClasses)
                {
                    var grades =
                        disciplineClass.Grades?
                            .ToList()
                        ?? new List<Grade>();

                    vm.Classes.Add(
                        new TeacherClassViewModel
                        {
                            DisciplineClassId =
                                disciplineClass.Id,

                            Discipline =
                                disciplineClass
                                    .Discipline?.Name ?? "-",

                            ClassCode =
                                disciplineClass
                                    .StudentClass?.Code ?? "-",

                            TeacherName =
                                disciplineClass
                                    .Teacher?.Name ?? "-",

                            StudentsCount =
                                grades
                                    .Select(g => g.RegistrationId)
                                    .Distinct()
                                    .Count(),

                            AverageGrade =
                                grades.Any()
                                    ? grades.Average(
                                        g => g.StudentGrade)
                                    : 0,

                            AverageFrequency =
                                grades.Any()
                                    ? grades.Average(
                                        g => g.Frequency)
                                    : 0
                        });
                }

                return vm;
            }

            // =========================================
            // PROFESSOR -> APENAS SUAS DISCIPLINAS
            // =========================================

            var teacher =
                await _context.Teachers
                    .Include(t => t.DisciplineClasses)
                        .ThenInclude(dc => dc.Discipline)
                    .Include(t => t.DisciplineClasses)
                        .ThenInclude(dc => dc.StudentClass)
                    .Include(t => t.DisciplineClasses)
                        .ThenInclude(dc => dc.Grades)
                    .FirstOrDefaultAsync(t =>
                        t.Email == email);

            if (teacher == null)
            {
                return null;
            }

            vm.TeacherName =
                teacher.Name ?? "Teacher";

            foreach (
                var disciplineClass in
                teacher.DisciplineClasses
                    ?? new List<DisciplineClass>())
            {
                var grades =
                    disciplineClass.Grades?
                        .ToList()
                    ?? new List<Grade>();

                vm.Classes.Add(
                    new TeacherClassViewModel
                    {
                        DisciplineClassId =
                            disciplineClass.Id,

                        Discipline =
                            disciplineClass
                                .Discipline?.Name ?? "-",

                        ClassCode =
                            disciplineClass
                                .StudentClass?.Code ?? "-",

                        TeacherName =
                            teacher.Name ?? "Teacher",

                        StudentsCount =
                            grades
                                .Select(g => g.RegistrationId)
                                .Distinct()
                                .Count(),

                        AverageGrade =
                            grades.Any()
                                ? grades.Average(
                                    g => g.StudentGrade)
                                : 0,

                        AverageFrequency =
                            grades.Any()
                                ? grades.Average(
                                    g => g.Frequency)
                                : 0
                    });
            }

            return vm;
        }
    }
}
