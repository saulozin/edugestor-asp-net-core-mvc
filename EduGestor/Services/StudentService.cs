using EduGestor.Data;
using EduGestor.Models;
using EduGestor.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class StudentService
    {
        private readonly EduGestorContext _context;

        public StudentService(EduGestorContext context)
        {
            _context = context;
        }

        // =========================
        // STUDENTS
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
                .Include(s => s.Grades)
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

        public async Task InsertAsync(Student student)
        {
            _context.Students.Add(student);

            await _context.SaveChangesAsync();
        }

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
