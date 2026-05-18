using EduGestor.Data;
using EduGestor.Models;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class GuardianService
    {
        private readonly EduGestorContext _context;

        public GuardianService(EduGestorContext context)
        {
            _context = context;
        }

        // =========================
        // GUARDIANS
        // =========================

        public async Task<List<Guardian>> FindAllAsync()
        {
            return await _context.Guardians
                .Include(g => g.Students)
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<Guardian?> FindByIdAsync(Guid id)
        {
            return await _context.Guardians
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Guardian?> FindByCpfAsync(string cpf)
        {
            return await _context.Guardians
                .FirstOrDefaultAsync(g => g.Cpf == cpf);
        }

        public async Task<Guardian> InsertAsync(Guardian guardian)
        {
            _context.Guardians.Add(guardian);

            await _context.SaveChangesAsync();

            return guardian;
        }

        public async Task UpdateAsync(Guardian guardian)
        {
            bool exists = await _context.Guardians
                .AnyAsync(g => g.Id == guardian.Id);

            if (!exists)
            {
                throw new ApplicationException("Guardian not found.");
            }

            try
            {
                _context.Update(guardian);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException err)
            {
                throw new DbUpdateConcurrencyException(err.Message);
            }
        }

        public async Task RemoveAsync(Guid id)
        {
            try
            {
                var guardian = await _context.Guardians
                    .Include(g => g.Students)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (guardian == null)
                {
                    throw new ApplicationException("Guardian not found.");
                }

                // remove associação dos students
                foreach (var student in guardian.Students)
                {
                    student.GuardianId = null;
                }

                _context.Guardians.Remove(guardian);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException err)
            {
                throw new DbUpdateException(err.Message);
            }
        }
    }
}
