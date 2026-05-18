using EduGestor.Data;
using EduGestor.Models;
using EduGestor.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class RegistrationService
    {
        private readonly EduGestorContext _context;

        public RegistrationService(EduGestorContext context)
        {
            _context = context;
        }

        public async Task<List<Registration>> FindAllAsync()
        {
            return await _context.Registrations
                .Include(r => r.Student)
                    .ThenInclude(s => s.Guardian)
                .Include(r => r.StudentClass)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        public async Task<Registration?> FindByIdAsync(Guid id)
        {
            return await _context.Registrations
                .Include(r => r.Student)
                .Include(r => r.StudentClass)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task InsertAsync(Registration reg)
        {
            _context.Registrations.Add(reg);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Registration reg)
        {
            bool exists = await _context.Registrations.AnyAsync(r => r.Id == reg.Id);

            if (!exists)
            {
                throw new NotFoundException("Teacher Id not found.");
            }

            try
            {
                _context.Registrations.Update(reg);

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
                var reg = await _context.Registrations.FindAsync(id);

                if (reg == null)
                {
                    throw new NotFoundException("Student Id not found.");
                }

                _context.Registrations.Remove(reg);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException err)
            {
                throw new DbConcurrencyException(err.Message);
            }
        }
    }
}
