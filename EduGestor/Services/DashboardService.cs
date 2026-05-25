using EduGestor.Data;
using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class DashboardService
    {
        private readonly EduGestorContext _context;

        public DashboardService(EduGestorContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var registrations =
    await _context.Registrations
        .Include(r => r.Grades)
        .ToListAsync();

            var grades =
                await _context.Grades
                    .ToListAsync();

            var schoolAverage =
                grades.Any()
                    ? grades.Average(g => g.StudentGrade)
                    : 0;

            var averageFrequency =
                grades.Any()
                    ? grades.Average(g => g.Frequency)
                    : 0;

            var approvedStudents =
                registrations.Count(r =>
                    r.Grades.Any() &&
                    r.Grades
                        .GroupBy(g => g.DisciplineClassId)
                        .All(group =>
                            r.IsApproved(group.Key ?? Guid.Empty)));

            var failedStudents =
                registrations.Count(r =>
                    r.Grades.Any() &&
                    r.Grades
                        .GroupBy(g => g.DisciplineClassId)
                        .Any(group =>
                            !r.IsApproved(group.Key ?? Guid.Empty)));

            var vm = new DashboardViewModel
            {
                TotalStudents = await _context.Students.CountAsync(),

                TotalGuardians = await _context.Guardians.CountAsync(),

                TotalTeachers = await _context.Teachers.CountAsync(),

                TotalRegistrations = await _context.Registrations.CountAsync(),

                TotalStudentClasses = await _context.StudentClasses.CountAsync(),

                SchoolAverage = schoolAverage,

                AverageFrequency = averageFrequency,

                ApprovedStudents = approvedStudents,

                FailedStudents = failedStudents,

                RecentStudents = await _context.Students
                    .Include(s => s.Guardian)
                    .OrderByDescending(s => s.Id)
                    .Take(5)
                    .ToListAsync(),

                RecentRegistrations = await _context.Registrations
                    .Include(r => r.Student)
                        .ThenInclude(s => s.Guardian)
                    .Include(r => r.StudentClass)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return vm;
        }
    }
}
