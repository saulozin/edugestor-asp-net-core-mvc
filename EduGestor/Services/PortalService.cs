using EduGestor.Data;
using EduGestor.Models.Enums;
using EduGestor.Models.ViewModels.Portal;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class PortalService
    {
        private readonly EduGestorContext _context;

        private readonly AcademicRulesService _academicRulesService;

        public PortalService(EduGestorContext context, AcademicRulesService academicRulesService)
        {
            _context = context;
            _academicRulesService = academicRulesService;
        }

        public async Task<PortalViewModel?> GetPortalDataAsync(string email)
        {
            var guardian = await _context.Guardians
                .Include(g => g.Students)
                    .ThenInclude(s => s.Registrations)
                        .ThenInclude(r => r.StudentClass)

                .Include(g => g.Students)
                    .ThenInclude(s => s.Registrations)
                        .ThenInclude(r => r.Grades)
                            .ThenInclude(g => g.DisciplineClass)
                                .ThenInclude(dc => dc!.Discipline)

                .Include(g => g.Students)
                    .ThenInclude(s => s.Registrations)
                        .ThenInclude(r => r.Attendances)

                .FirstOrDefaultAsync(g => g.Email == email);

            if (guardian == null)
            {
                return null;
            }

            var vm = new PortalViewModel
            {
                GuardianName = guardian.Name
            };

            foreach (var student in guardian.Students)
            {
                var registration =
                    student.Registrations
                        .OrderByDescending(r => r.CreatedAt)
                        .FirstOrDefault();

                if (registration == null)
                    continue;

                var studentVm = new PortalStudentViewModel
                {
                    RegistrationId = registration.Id,

                    StudentId = student.Id,

                    StudentName = student.Name,

                    ClassCode =
                        registration.StudentClass?.Code ?? "-",

                     GuardianName = guardian.Name
                };

                foreach (var grade in registration.Grades)
                {
                    var frequency =
                        registration.GetAttendance(
                            grade.DisciplineClassId ?? Guid.Empty);

                    var status =
                        _academicRulesService.CalculateStatus(
                            grade.StudentGrade, frequency);

                    studentVm.Grades.Add(
                        new PortalGradeViewModel
                        {
                            Discipline =
                                grade.DisciplineClass?
                                    .Discipline?.Name ?? "-",

                            Grade = grade.StudentGrade,

                            Bimester = grade.Bimester,

                            Frequency = frequency,

                            Status = status
                        });
                }

                vm.Students.Add(studentVm);
            }

            return vm;
        }
    }
}
