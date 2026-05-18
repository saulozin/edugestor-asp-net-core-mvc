using EduGestor.Data;
using EduGestor.Models;
using EduGestor.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class StudentClassService
    {
        private readonly EduGestorContext _context;

        public StudentClassService(EduGestorContext context)
        {
            _context = context;
        }

        public async Task<List<StudentClass>> FindAllAsync()
        {
            return await _context.StudentClasses
                .OrderBy(sc => sc.Level)
                .ThenBy(sc => sc.Series)
                .ThenBy(sc => sc.Shift)
                .ToListAsync();
        }

        public async Task<StudentClass?> FindByIdAsync(Guid id)
        {
            return await _context.StudentClasses
                .Include(sc => sc.Registrations)
                .Include(sc => sc.DisciplineClasses)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task InsertAsync(StudentClass obj)
        {
            _context.StudentClasses.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StudentClass sClass)
        {
            bool exists = await _context.StudentClasses.AnyAsync(sc => sc.Id == sClass.Id);

            if (!exists)
            {
                throw new NotFoundException("Student Class Id not found.");
            }

            try
            {
                _context.StudentClasses.Update(sClass);

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
                var sClass = await _context.StudentClasses.FindAsync(id);

                if (sClass == null)
                {
                    throw new NotFoundException("Student Class Id not found.");
                }

                _context.StudentClasses.Remove(sClass);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException err)
            {
                throw new DbConcurrencyException(err.Message);
            }
        }
    }
}
