using EduGestor.Data;
using EduGestor.Extensions;
using EduGestor.Models;
using EduGestor.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EduGestor.Services
{
    public class GuardianService
    {
        private readonly EduGestorContext _context;
        private readonly ValidateExtensions _validate;

        public GuardianService(EduGestorContext context, ValidateExtensions validate)
        {
            _context = context;
            _validate = validate;
        }

        // =========================
        // GUARDIANS
        // =========================
        public async Task<List<Guardian>> FindAllSearchAsync(string? searchString)
        {
            var query = _context.Guardians
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();

                // Remove máscara do CPF digitado
                var normalizedSearch = searchString.OnlyNumbers();
                    

                query = query.Where(g =>
                    // Nome (case insensitive)
                    EF.Functions.ILike(g.Name, $"%{searchString}%") ||

                    // Email (case insensitive)
                    EF.Functions.ILike(g.Email, $"%{searchString}%") ||

                    // CPF
                    (!string.IsNullOrEmpty(normalizedSearch) &&

                        g.Cpf.Replace(".", "")
                            .Replace("-", "")
                            .Contains(normalizedSearch))
                );
            }

            return await query
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

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

        // =====================
        // CREATE
        // =====================
        public async Task<Guardian> InsertAsync(Guardian guardian)
        {
            if (await _validate.ExistsAsync<Guardian>(g => g.Id == guardian.Id))
            {
                throw new IntegrityException(
                    "This guardian already exists.");
            }

            if (await _validate.ExistsAsync<Guardian>(g => g.Cpf == guardian.Cpf))
            {
                throw new IntegrityException(
                    "This CPF already exists.");
            }

            guardian.CreatedAt = DateTime.UtcNow;

            _context.Guardians.Add(guardian);

            await _context.SaveChangesAsync();

            return guardian;
        }

        // =====================
        // UPDATE
        // =====================
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
                var original = await _context.Guardians
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Id == guardian.Id);

                guardian.Cpf = original.Cpf;

                guardian.UpdatedAt = DateTime.UtcNow;

                _context.Guardians.Update(guardian);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException err)
            {
                throw new DbUpdateConcurrencyException(err.Message);
            }
        }

        // =====================
        // DELETE
        // =====================
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
