using EduGestor.Data;
using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class GradeService
    {
        private readonly EduGestorContext _context;

        public GradeService(EduGestorContext context)
        {
            _context = context;
        }

        public async Task<List<Grade>> FindAllAsync()
        {
            return await _context.Grades

                // REGISTRATION
                .Include(g => g.Registration)
                    .ThenInclude(r => r.Student)

                .Include(g => g.Registration)
                    .ThenInclude(r => r.StudentClass)

                // DISCIPLINE CLASS
                .Include(g => g.DisciplineClass)
                    .ThenInclude(dc => dc.Discipline)

                .Include(g => g.DisciplineClass)
                    .ThenInclude(dc => dc.Teacher)

                .Include(g => g.DisciplineClass)
                    .ThenInclude(dc => dc.StudentClass)

                .OrderByDescending(g => g.CreatedAt)

                .ToListAsync();
        }

        public async Task<GradeSearchViewModel> FindAllSearchAsync(GradeSearchViewModel filters)
        {
            var query = _context.Grades
                .Include(g => g.Registration)
                    .ThenInclude(r => r!.Student)
                .Include(g => g.Registration)
                    .ThenInclude(r => r!.StudentClass)
                .Include(g => g.DisciplineClass)
                    .ThenInclude(dc => dc!.Discipline)
                .Include(g => g.DisciplineClass)
                    .ThenInclude(dc => dc!.Teacher)
                .AsQueryable();

            // Global Search
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var search = filters.Search.Trim();

                query = query.Where(g =>

                    EF.Functions.ILike(
                        g.Registration!.Student!.Name,
                        $"%{search}%")

                    ||

                    EF.Functions.ILike(
                        g.Registration.StudentClass!.Code,
                        $"%{search}%")

                    ||

                    EF.Functions.ILike(
                        g.DisciplineClass!.Discipline!.Name,
                        $"%{search}%")

                    ||

                    EF.Functions.ILike(
                        g.DisciplineClass.Teacher!.Name,
                        $"%{search}%")
                );
            }

            // Student grades
            if (filters.MinGrade.HasValue)
            {
                query = query.Where(g =>
                    g.StudentGrade >= filters.MinGrade.Value);
            }

            if (filters.MaxGrade.HasValue)
            {
                query = query.Where(g =>
                    g.StudentGrade <= filters.MaxGrade.Value);
            }

            // Student Frequancies
            if (filters.MinFrequency.HasValue)
            {
                query = query.Where(g =>
                    g.Frequency >= filters.MinFrequency.Value);
            }

            // Bimester
            if (filters.Bimester.HasValue)
            {
                query = query.Where(g => g.Bimester == filters.Bimester);
            }

            // School Year
            if (filters.SchoolYear.HasValue)
            {
                query = query.Where(g => g.SchoolYear == filters.SchoolYear);
            }

            // Student Status
            if (filters.Approved.HasValue)
            {
                if (filters.Approved.Value)
                {
                    // APPROVED
                    query = query.Where(g =>
                        g.StudentGrade >= 6.0m &&
                        g.Frequency >= 75.0m);
                }
                else
                {
                    // FAILED
                    query = query.Where(g =>
                        g.StudentGrade < 6.0m ||
                        g.Frequency < 75.0m);
                }
            }

            // TOTAL
            var totalItems = await query.CountAsync();

            // PAGINATION
            var grades = await query
                .OrderBy(g => g.Registration!.Student!.Name)
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            filters.Grades = grades;

            filters.TotalPages =
                (int)Math.Ceiling(totalItems / (double)filters.PageSize);

            return filters;
        }

        public async Task<Grade?> FindByIdAsync(Guid id)
        {
            return await _context.Grades

                .Include(g => g.Registration)
                    .ThenInclude(r => r.Student)

                .Include(g => g.Registration)
                    .ThenInclude(r => r.StudentClass)

                .Include(g => g.DisciplineClass)
                    .ThenInclude(dc => dc.Discipline)

                .Include(g => g.DisciplineClass)
                    .ThenInclude(dc => dc.Teacher)

                .FirstOrDefaultAsync(g => g.Id == id);
        }

        private async Task ValidateGradeRules(Grade grade)
        {
            var registration = await _context.Registrations
                .Include(r => r.StudentClass)
                .FirstOrDefaultAsync(r => r.Id == grade.RegistrationId);

            var disciplineClass = await _context.DisciplineClasses
                .FirstOrDefaultAsync(dc => dc.Id == grade.DisciplineClassId);

            if (registration == null || disciplineClass == null)
            {
                throw new NotFoundException("Invalid data.");
            }

            if (registration.StudentClassId != disciplineClass.StudentClassId)
            {
                throw new IntegrityException(
                    "The selected discipline does not belong to the student's class.");
            }
        }

        // ========================
        // CREATE
        // ========================
        public async Task InsertAsync(Grade grade)
        {
            bool alreadyExists = await _context.Grades
                .AnyAsync(g =>
                    g.RegistrationId == grade.RegistrationId &&
                    g.DisciplineClassId == grade.DisciplineClassId &&
                    g.Bimester == grade.Bimester);

            if (alreadyExists)
            {
                throw new IntegrityException(
                    "A grade already exists for this bimester and discipline.");
            }

            await ValidateGradeRules(grade);

            grade.CreatedAt = DateTime.UtcNow;

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
        }

        // ========================
        // EDIT
        // ========================
        public async Task UpdateAsync(Grade grade)
        {
            bool exists = await _context.Grades
               .AnyAsync(g => g.Id == grade.Id);

            if (!exists)
            {
                throw new NotFoundException(
                    "Grade Id not found.");
            }

            bool duplicated = await _context.Grades
                .AnyAsync(g =>
                    g.Id != grade.Id &&
                    g.RegistrationId == grade.RegistrationId &&
                    g.DisciplineClassId == grade.DisciplineClassId &&
                    g.Bimester == grade.Bimester);

            if (duplicated)
            {
                throw new IntegrityException(
                    "A grade already exists for this bimester and discipline.");
            }

            await ValidateGradeRules(grade);

            try
            {
                grade.UpdatedAt = DateTime.UtcNow;

                _context.Grades.Update(grade);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException err)
            {
                throw new DbConcurrencyException(
                    err.Message);
            }
        }

        // =====================
        // DELETE
        // =====================
        public async Task RemoveAsync(Guid id)
        {
            try
            {
                var grade = await _context.Grades.FindAsync(id);

                if (grade == null)
                {
                    throw new NotFoundException("Grade Id not found.");
                }

                _context.Grades.Remove(grade);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException err)
            {
                throw new DbConcurrencyException(err.Message);
            }
        }
    }
}
