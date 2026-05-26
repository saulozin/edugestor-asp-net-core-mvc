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
                bool isAdmin)
        {
            Teacher? teacher = null;

            // =========================
            // TEACHER USER
            // =========================

            if (!isAdmin)
            {
                teacher =
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
            }

            // =========================
            // ADMIN USER
            // =========================

            var disciplineClassesQuery =
                _context.DisciplineClasses
                    .Include(dc => dc.Discipline)
                    .Include(dc => dc.StudentClass)
                    .Include(dc => dc.Grades)
                    .AsQueryable();

            // FILTER ONLY FOR TEACHER
            if (!isAdmin)
            {
                disciplineClassesQuery =
                    disciplineClassesQuery
                        .Where(dc =>
                            dc.TeacherId == teacher!.Id);
            }

            var disciplineClasses =
                await disciplineClassesQuery
                    .ToListAsync();

            var vm =
                new TeacherPortalViewModel
                {
                    TeacherName =
                        isAdmin
                            ? "Administrator"
                            : teacher!.Name
                };

            foreach (var disciplineClass in disciplineClasses)
            {
                var grades =
                    disciplineClass.Grades.ToList();

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
