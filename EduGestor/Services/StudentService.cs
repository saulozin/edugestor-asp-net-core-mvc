using EduGestor.Data;
using EduGestor.Extensions;
using EduGestor.Models;
using EduGestor.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class StudentService
    {
        private readonly EduGestorContext _context;
        private readonly ValidateExtensions _validate;

        public StudentService(EduGestorContext context, ValidateExtensions validate)
        {
            _context = context;
            _validate = validate;
        }

        // =========================
        // STUDENTS LIST
        // =========================

        public async Task<List<Student>> FindAllAsync()
        {
            return await _context.Students
                .Include(s => s.Guardian)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<List<Student>> FindAllWithJoinAsync()
        {
            return await _context.Students
                .Include(s => s.Guardian)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.StudentClass)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Grades)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Student?> FindByIdAsync(Guid id)
        {
            return await _context.Students
                .Include(s => s.Guardian)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.StudentClass)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Grades)
                .OrderBy(s => s.Name)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // =========================
        // STUDENTS CREATE
        // =========================
        public async Task InsertAsync(Student student)
        {
            if (await _validate.ExistsAsync<Student>(s => s.Id == student.Id))
            {
                throw new IntegrityException(
                    "This student already exists.");
            }

            if (await _validate.ExistsAsync<Student>(s => s.Cpf == student.Cpf))
            {
                throw new IntegrityException(
                    "This CPF already exists.");
            }

            student.CreatedAt = DateTime.UtcNow;

            _context.Students.Add(student);

            await _context.SaveChangesAsync();
        }

        // =========================
        // STUDENTS EDIT
        // =========================

        public async Task<Student?> FindByIdForUpdateAsync(Guid id)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(Student student)
        {
            bool exists = await _context.Students
                .AnyAsync(s => s.Id == student.Id);

            if (!exists)
            {
                throw new NotFoundException("Student not found.");
            }

            try
            {
                var original = await _context.Students
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == student.Id);

                student.Cpf = original.Cpf;

                student.UpdatedAt = DateTime.UtcNow;

                _context.Students.Update(student);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException err)
            {
                throw new DbConcurrencyException(err.Message);
            }
        }

        public async Task RemoveAsync(Guid id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);

                if (student == null)
                {
                    throw new NotFoundException("Student Id not found.");
                }

                _context.Students.Remove(student);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException err)
            {
                throw new DbConcurrencyException(err.Message);
            }
        }
    }
}
