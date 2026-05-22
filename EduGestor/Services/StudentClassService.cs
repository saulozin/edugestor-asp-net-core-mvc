using EduGestor.Data;
using EduGestor.Extensions;
using EduGestor.Models;
using EduGestor.Models.ViewModels;
using EduGestor.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class StudentClassService
    {
        private readonly EduGestorContext _context;
        private readonly ValidateExtensions _validate;

        public StudentClassService(EduGestorContext context, ValidateExtensions validate)
        {
            _context = context;
            _validate = validate;
        }

        public async Task<List<StudentClass>> FindAllAsync()
        {
            return await _context.StudentClasses
                .OrderBy(sc => sc.Level)
                .ThenBy(sc => sc.Series)
                .ThenBy(sc => sc.Shift)
                .ToListAsync();
        }

        public async Task<StudentClassSearchViewModel> FindAllSearchAsync(StudentClassSearchViewModel filters)
        {
            var query = _context.StudentClasses
                .Include(sc => sc.Registrations)
                .Include(sc => sc.DisciplineClasses)
                .AsQueryable();

            // Education Level
            if (filters.EduLevel.HasValue)
            {
                query = query.Where(sc => sc.Level == filters.EduLevel);
            }

            // Student Series
            if (filters.StudentSeries.HasValue)
            {
                query = query.Where(sc => sc.Series == filters.StudentSeries);
            }

            // Student Shift
            if (filters.ClassShift.HasValue)
            {
                query = query.Where(sc => sc.Shift == filters.ClassShift);
            }

            // Class Code
            if (!string.IsNullOrWhiteSpace(filters.ClassCode))
            {
                query = query.Where(sc =>
                    EF.Functions.ILike(sc.Code, $"%{filters.ClassCode}%")
                );
            }

            // TOTAL ITEMS
            var totalItems = await query.CountAsync();

            // PAGINATION
            var scItems = await query
                .OrderByDescending(sc => sc.Level)
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            filters.StudentClasses = scItems;

            filters.TotalPages =
                (int)Math.Ceiling(
                    totalItems / (double)filters.PageSize);

            return filters;
        }

        public async Task<StudentClass?> FindByIdAsync(Guid id)
        {
            return await _context.StudentClasses
                .Include(sc => sc.Registrations)
                .Include(sc => sc.DisciplineClasses)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task InsertAsync(StudentClass sc)
        {
            if (await _validate.ExistsAsync<StudentClass>(obj => obj.Id == sc.Id))
            {
                throw new IntegrityException(
                    "This student class already exists.");
            }

            if (await _validate.ExistsAsync<StudentClass>(obj => obj.Code == sc.Code))
            {
                throw new IntegrityException(
                    "This code already exists.");
            }

            sc.CreatedAt = DateTime.UtcNow;

            _context.StudentClasses.Add(sc);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StudentClass sClass)
        {
            bool exists = await _context.StudentClasses
                .AnyAsync(sc => sc.Id == sClass.Id);

            if (!exists)
            {
                throw new NotFoundException(
                    "Student Class Id not found.");
            }

            bool codeExists = await _context.StudentClasses
                .AnyAsync(sc =>
                    sc.Code == sClass.Code &&
                    sc.Id != sClass.Id);

            if (codeExists)
            {
                throw new IntegrityException(
                    "This code already exists.");
            }

            try
            {
                sClass.UpdatedAt = DateTime.UtcNow;

                _context.StudentClasses.Update(sClass);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException err)
            {
                throw new DbConcurrencyException(
                    err.Message);
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
