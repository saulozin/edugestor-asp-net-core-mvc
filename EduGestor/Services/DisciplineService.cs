using EduGestor.Data;
using EduGestor.Extensions;
using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace EduGestor.Services
{
    public class DisciplineService
    {
        private readonly EduGestorContext _context;

        public DisciplineService(EduGestorContext context)
        {
            _context = context;
        }

        public async Task<List<Discipline>> FindAllAsync()
        {
            return await _context.Disciplines
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<PagedViewModel<Discipline>> FindAllSearchAsync(PagedViewModel<Discipline> filters)
        {
            var query = _context.Disciplines
                .Include(d => d.DisciplineClasses)
                .Include(d => d.Grades)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                filters.SearchTerm = filters.SearchTerm.Trim();

                query = query.Where(d =>
                    // Nome (case insensitive)
                    EF.Functions.ILike(d.Name, $"%{filters.SearchTerm}%")
                );
            }

            // TOTAL ITEMS
            var totalItems = await query.CountAsync();

            // PAGINATION
            var disciplines = await query
                .OrderBy(d => d.Name)
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            filters.Items = disciplines;

            filters.TotalPages =
                (int)Math.Ceiling(
                    totalItems / (double)filters.PageSize);

            return filters;
        }

        public async Task<Discipline?> FindByIdAsync(Guid id)
        {
            return await _context.Disciplines
                .Include(d => d.DisciplineClasses)
                .Include(d => d.Grades)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        // ========================
        // CREATE
        // ========================
        public async Task InsertAsync(Discipline disc)
        {
            disc.CreatedAt = DateTime.UtcNow;

            _context.Disciplines.Add(disc);
            await _context.SaveChangesAsync();
        }

        // ========================
        // EDIT
        // ========================
        public async Task UpdateAsync(Discipline disc)
        {
            bool exists = await _context.Disciplines
               .AnyAsync(d => d.Id == disc.Id);

            if (!exists)
            {
                throw new NotFoundException(
                    "Discipline Id not found.");
            }

            try
            {
                disc.UpdatedAt = DateTime.UtcNow;

                _context.Disciplines.Update(disc);

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
                var disc = await _context.Disciplines.FindAsync(id);

                if (disc == null)
                {
                    throw new NotFoundException("Discipline Id not found.");
                }

                _context.Disciplines.Remove(disc);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException err)
            {
                throw new DbConcurrencyException(err.Message);
            }
        }
    }
}
