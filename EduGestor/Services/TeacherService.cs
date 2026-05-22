using EduGestor.Data;
using EduGestor.Extensions;
using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class TeacherService
    {
        private readonly EduGestorContext _context;
        private readonly ValidateExtensions _validate;

        public TeacherService(EduGestorContext context, ValidateExtensions validate)
        {
            _context = context;
            _validate = validate;
        }

        public async Task<PagedViewModel<Teacher>> FindAllSearchAsync(PagedViewModel<Teacher> filters)
        {
            var query = _context.Teachers
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                filters.SearchTerm = filters.SearchTerm.Trim();

                // Remove máscara do CPF digitado
                var normalizedSearch = filters.SearchTerm.OnlyNumbers();


                query = query.Where(t =>
                    // Nome (case insensitive)
                    EF.Functions.ILike(t.Name, $"%{filters.SearchTerm}%") ||

                    // Email (case insensitive)
                    EF.Functions.ILike(t.Email, $"%{filters.SearchTerm}%") ||

                    // CPF
                    (!string.IsNullOrEmpty(normalizedSearch) &&

                        t.Cpf.Replace(".", "")
                            .Replace("-", "")
                            .Contains(normalizedSearch))
                );
            }

            // TOTAL ITEMS
            var totalItems = await query.CountAsync();

            // PAGINATION
            var teachers = await query
                .OrderBy(t => t.Name)
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            filters.Items = teachers;

            filters.TotalPages =
                (int)Math.Ceiling(
                    totalItems / (double)filters.PageSize);

            return filters;
        }

        public async Task<List<Teacher>> FindAllAsync()
        {
            return await _context.Teachers
                .OrderBy(t => t.Name)
                .ToListAsync();      
        }

        public async Task<Teacher?> FindByIdAsync(Guid id)
        {
            return await _context.Teachers.FirstOrDefaultAsync(obj => obj.Id == id);
        }

        // =====================
        // CREATE
        // =====================
        public async Task InsertAsync(Teacher teacher)
        {
            if (await _validate.ExistsAsync<Teacher>(t => t.Id == teacher.Id))
            {
                throw new IntegrityException(
                    "This teacher already exists.");
            }

            if (await _validate.ExistsAsync<Teacher>(t => t.Cpf == teacher.Cpf))
            {
                throw new IntegrityException(
                    "This CPF already exists.");
            }

            teacher.CreatedAt = DateTime.UtcNow;
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
        }

        // =====================
        // UPDATE
        // =====================
        public async Task UpdateAsync(Teacher teacher)
        {
            bool exists = await _context.Teachers.AnyAsync(t => t.Id == teacher.Id);

            if (!exists)
            {
                throw new NotFoundException("Teacher Id not found.");
            }

            try
            {
                var original = await _context.Teachers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == teacher.Id);

                teacher.Cpf = original.Cpf;

                teacher.UpdatedAt = DateTime.UtcNow;

                _context.Teachers.Update(teacher);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException err)
            {
                throw new DbConcurrencyException(err.Message);
            }
        }

        // ======================
        // DELETE
        // ======================
        public async Task RemoveAsync(Guid id)
        {
            try
            {
                var teacher = await _context.Teachers.FindAsync(id);

                if (teacher == null)
                {
                    throw new NotFoundException("Teacher Id not found.");
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
