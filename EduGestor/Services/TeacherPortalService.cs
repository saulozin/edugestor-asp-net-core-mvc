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

        private readonly AcademicRulesService _academicRulesService;

        public TeacherPortalService(EduGestorContext context, AcademicRulesService academicRulesService)
        {
            _context = context;
            _academicRulesService = academicRulesService;
        }

        public async Task<TeacherPortalViewModel?> GetPortalDataAsync(string email, bool isAdmin = false)
        {
            var vm = new TeacherPortalViewModel();

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

                    var registrationIds =
                        grades.Select(g => g.RegistrationId)
                        .Distinct()
                        .ToList();

                    decimal averageFrequency = 0;

                    if (registrationIds.Any())
                    {
                        var frequencies = new List<decimal>();

                        foreach (var registrationId in registrationIds)
                        {
                            var freq =
                                await CalculateFrequencyAsync(
                                    registrationId!.Value,
                                    disciplineClass.Id);

                            frequencies.Add(freq);
                        }

                        averageFrequency = frequencies.Average();
                    }

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

                            AverageFrequency = averageFrequency
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

            vm.TeacherName = teacher.Name ?? "Teacher";

            foreach (
                var disciplineClass in teacher.DisciplineClasses
                    ?? new List<DisciplineClass>())
            {
                var grades =
                    disciplineClass.Grades?.ToList()
                        ?? new List<Grade>();

                var registrationIds =
                        grades.Select(g => g.RegistrationId)
                        .Distinct()
                        .ToList();

                decimal averageFrequency = 0;

                if (registrationIds.Any())
                {
                    var frequencies = new List<decimal>();

                    foreach (var registrationId in registrationIds)
                    {
                        var freq =
                            await CalculateFrequencyAsync(
                                registrationId!.Value,
                                disciplineClass.Id);

                        frequencies.Add(freq);
                    }

                    averageFrequency = frequencies.Average();
                }

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

                        AverageFrequency = averageFrequency
                    });
            }

            return vm;
        }

        public async Task<TeacherClassDetailsViewModel?> GetClassDetailsAsync(Guid disciplineClassId)
        {
            var disciplineClass =
                await _context.DisciplineClasses
                    .Include(dc => dc.Discipline)
                    .Include(dc => dc.StudentClass)
                    .Include(dc => dc.Teacher)

                    .Include(dc => dc.Grades)
                        .ThenInclude(g => g.Registration)
                            .ThenInclude(r => r.Student)

                    .FirstOrDefaultAsync(dc =>
                        dc.Id == disciplineClassId);

            if (disciplineClass == null)
            {
                return null;
            }

            var vm =
                new TeacherClassDetailsViewModel
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
                            .Teacher?.Name ?? "-"
                };

            var registrations =
                disciplineClass.Grades
                    .Select(g => g.Registration)
                    .Where(r => r != null)
                    .Distinct()
                    .ToList();

            foreach (var registration in registrations)
            {
                var studentGrades =
                    disciplineClass.Grades
                        .Where(g =>
                            g.RegistrationId ==
                            registration!.Id)
                        .ToList();

                var averageGrade =
                    studentGrades.Any()
                        ? studentGrades.Average(
                            g => g.StudentGrade)
                        : 0;

                var frequency =
                    await CalculateFrequencyAsync(
                        registration!.Id, disciplineClass.Id);

                vm.Students.Add(
                    new TeacherStudentRowViewModel
                    {
                        RegistrationId =
                            registration!.Id,

                        StudentName =
                            registration.Student?.Name
                            ?? "-",

                        AverageGrade =
                            averageGrade,

                        Frequency =
                            frequency,

                        Status = _academicRulesService
                            .CalculateStatus(averageGrade, frequency)

                    });
            }

            return vm;
        }

        public async Task<GradeLaunchViewModel?> GetLaunchGradesDataAsync(Guid disciplineClassId, int bimester)
        {
            var disciplineClass =
                await _context.DisciplineClasses

                    .Include(dc => dc.Discipline)

                    .Include(dc => dc.StudentClass)
                        .ThenInclude(sc => sc.Registrations)
                            .ThenInclude(r => r.Student)

                    .Include(dc => dc.Grades)

                    .FirstOrDefaultAsync(dc =>
                        dc.Id == disciplineClassId);

            if (disciplineClass == null)
            {
                return null;
            }

            var vm =
                new GradeLaunchViewModel
                {
                    DisciplineClassId =
                        disciplineClass.Id,

                    Discipline =
                        disciplineClass
                            .Discipline?.Name ?? "-",

                    ClassCode =
                        disciplineClass
                            .StudentClass?.Code ?? "-",

                    Bimester = bimester
                };

            var registrations =
                disciplineClass
                    .StudentClass?
                    .Registrations
                    ?.ToList()

                ?? new List<Registration>();

            foreach (var registration in registrations)
            {
                // procura nota já existente
                var existingGrade =
                    disciplineClass.Grades
                        .FirstOrDefault(g =>
                            g.RegistrationId ==
                                registration.Id
                            &&
                            g.Bimester == bimester);

                vm.Students.Add(
                    new GradeLaunchRowViewModel
                    {
                        RegistrationId =
                            registration.Id,

                        StudentName =
                            registration.Student?.Name
                            ?? "-",

                        Grade =
                            existingGrade?.StudentGrade
                            ?? 0,
                    });
            }

            return vm;
        }

        public async Task SaveLaunchGradesAsync(GradeLaunchViewModel vm)
        {
            foreach (var student in vm.Students)
            {
                var existingGrade =
                    await _context.Grades
                        .FirstOrDefaultAsync(g =>
                            g.RegistrationId == student.RegistrationId &&
                            g.DisciplineClassId == vm.DisciplineClassId &&
                            g.Bimester == vm.Bimester);

                var frequency =
                    await CalculateFrequencyAsync(
                        student.RegistrationId, vm.DisciplineClassId);

                // =========================================
                // UPDATE
                // =========================================

                if (existingGrade != null)
                {
                    existingGrade.StudentGrade = student.Grade;

                    existingGrade.UpdatedAt = DateTime.UtcNow;

                    continue;
                }

                // =========================================
                // CREATE
                // =========================================

                var grade =
                    new Grade
                    {
                        RegistrationId = student.RegistrationId,

                        DisciplineClassId = vm.DisciplineClassId,

                        StudentGrade = student.Grade,

                        Bimester = vm.Bimester
                    };

                _context.Grades.Add(grade);
            }

            await _context.SaveChangesAsync();
        }

        // =========================
        // Attendance
        // =========================
        public async Task<AttendanceLaunchViewModel?> GetLaunchAttendanceDataAsync(
            Guid disciplineClassId, DateOnly date)
        {
            var disciplineClass = await _context.DisciplineClasses
                .Include(dc => dc.Discipline)
                .Include(dc => dc.StudentClass)
                    .ThenInclude(sc => sc!.Registrations)
                        .ThenInclude(r => r.Student)
                .Include(dc => dc.Attendances)
                .FirstOrDefaultAsync(dc => dc.Id == disciplineClassId);

            if (disciplineClass == null)
            {
                return null;
            }

            var vm =
                new AttendanceLaunchViewModel
                {
                    DisciplineClassId = disciplineClass.Id,

                    Discipline = disciplineClass.Discipline?.Name ?? "-",

                    ClassCode = disciplineClass.StudentClass?.Code ?? "-",

                    Date = date
                };

            var registrations = 
                disciplineClass
                    .StudentClass?
                    .Registrations?
                    .ToList()

                ?? new List<Registration>();

            foreach (var registration in registrations)
            {
                var existingAttendance = disciplineClass.Attendances
                    .FirstOrDefault(a =>
                        a.RegistrationId == registration.Id &&
                        a.Date == date);

                vm.Students.Add(
                    new AttendanceLaunchRowViewModel
                    {
                        RegistrationId = registration.Id,

                        StudentName = registration.Student?.Name ?? "-",

                        Present = existingAttendance?.Present ?? true
                    });
            }

            return vm;
        }

        public async Task SaveLaunchAttendanceAsync(AttendanceLaunchViewModel vm)
        {
            foreach (var student in vm.Students)
            {
                var existingAttendance = await _context.Attendances
                    .FirstOrDefaultAsync(a =>
                        a.RegistrationId == student.RegistrationId &&
                        a.DisciplineClassId == vm.DisciplineClassId &&
                        a.Date == vm.Date);

                // =========================
                // UPDATE
                // =========================

                if (existingAttendance != null)
                {
                    existingAttendance.Present = student.Present;
                    existingAttendance.UpdatedAt = DateTime.UtcNow;

                    continue;
                }

                // =========================
                // CREATE
                // =========================

                var attendance =
                    new Attendance
                    {
                        RegistrationId = student.RegistrationId,

                        DisciplineClassId = vm.DisciplineClassId,

                        Date = vm.Date,

                        Present = student.Present
                    };

                _context.Attendances.Add(attendance);
            }

            await _context.SaveChangesAsync();
        }

        private async Task<decimal> CalculateFrequencyAsync(Guid registrationId, Guid disciplineClassId)
        {
            var attendances =
                await _context.Attendances
                    .Where(a =>
                        a.RegistrationId == registrationId
                        &&
                        a.DisciplineClassId == disciplineClassId)
                    .ToListAsync();

            if (!attendances.Any())
            {
                return 0;
            }

            var totalClasses =
                attendances.Count;

            var presents =
                attendances.Count(a => a.Present);

            return
                Math.Round(
                    ((decimal)presents / totalClasses) * 100,
                    2);
        }

    }
}
