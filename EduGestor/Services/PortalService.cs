using EduGestor.Data;
using EduGestor.Models.ViewModels.Portal;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class PortalService
    {
        private readonly EduGestorContext _context;

        public PortalService(EduGestorContext context)
        {
            _context = context;
        }

        public async Task<PortalViewModel?> GetPortalDataAsync(
            string email)
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
                    studentVm.Grades.Add(
                        new PortalGradeViewModel
                        {
                            Discipline =
                                grade.DisciplineClass?
                                    .Discipline?.Name ?? "-",

                            Grade = grade.StudentGrade,

                            Frequency = grade.Frequency,

                            Bimester = grade.Bimester,

                            Approved =
                                grade.StudentGrade >= 6m &&
                                grade.Frequency >= 75m
                        });
                }

                vm.Students.Add(studentVm);
            }

            return vm;
        }
    }
}
