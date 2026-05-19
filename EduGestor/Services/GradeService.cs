using EduGestor.Data;
using EduGestor.Models;
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
