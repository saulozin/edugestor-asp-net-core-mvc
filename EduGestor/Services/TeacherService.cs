using EduGestor.Data;
using EduGestor.Models;
using EduGestor.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class TeacherService
    {
        private readonly EduGestorContext _context;

        public TeacherService(EduGestorContext context)
        {
            _context = context;
        }

        public async Task<List<Teacher>> FindAllAsync()
        {
            return await _context.Teachers
                .OrderBy(t => t.Name)
                .ToListAsync();      
        }

        public async Task<Teacher> FindByIdAsync(Guid? id)
        {
            return await _context.Teachers.FirstOrDefaultAsync(obj => obj.Id == id);
        }

        public async Task InsertAsync(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Teacher teacher)
        {
            bool exists = await _context.Teachers.AnyAsync(t => t.Id == teacher.Id);

            if (!exists)
            {
                throw new NotFoundException("Teacher Id not found.");
            }

            try
            {
                _context.Teachers.Update(teacher);

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
                var teacher = await _context.Teachers.FindAsync(id);

                if (teacher == null)
                {
                    throw new NotFoundException("Student Id not found.");
                }

                _context.Teachers.Remove(teacher);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException err)
            {
                throw new DbConcurrencyException(err.Message);
            }
        }
    }
}
