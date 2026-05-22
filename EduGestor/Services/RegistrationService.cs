using EduGestor.Data;
using EduGestor.Extensions;
using EduGestor.Models;
using EduGestor.Models.Enums;
using EduGestor.Models.ViewModels;
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

        public async Task<RegistrationSearchViewModel> FindAllSearchAsync(RegistrationSearchViewModel filters)
        {
            var query = _context.Registrations
                .Include(r => r.Student)
                    .ThenInclude(s => s!.Guardian)
                .Include(r => r.StudentClass)
                .AsQueryable();

            // STUDENT NAME
            if (!string.IsNullOrWhiteSpace(filters.StudentName))
            {
                query = query.Where(r =>
                    EF.Functions.ILike(r.Student!.Name, $"%{filters.StudentName}%")
                );
            }

            // STATUS
            if (filters.Status.HasValue)
            {
                query = query.Where(r => r.Status == filters.Status.Value);
            }

            // START DATE
            if (filters.StartDate.HasValue)
            {
                query = query.Where(r => r.Date >= filters.StartDate.Value);
            }

            // END DATE
            if (filters.EndDate.HasValue)
            {
                query = query.Where(r => r.Date <= filters.EndDate.Value);
            }

            // CLASS
            if (filters.StudentClassId.HasValue)
            {
                query = query.Where(r => r.StudentClassId == filters.StudentClassId);
            }

            // TOTAL ITEMS
            var totalItems = await query.CountAsync();

            // PAGINATION
            var regs = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            filters.Registrations = regs;

            filters.TotalPages =
                (int)Math.Ceiling(
                    totalItems / (double)filters.PageSize);

            return filters;
        }

        public async Task<List<Registration>> FindAllAsync()
        {
            return await _context.Registrations
                .Include(r => r.Student)
                    .ThenInclude(s => s!.Guardian)
                .Include(r => r.StudentClass)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Registration?> FindByIdAsync(Guid id)
        {
            return await _context.Registrations

                .Include(r => r.Student)
                .Include(r => r.StudentClass)

                .Include(r => r.Grades)
                    .ThenInclude(g => g.DisciplineClass)
                        .ThenInclude(dc => dc!.Discipline)

                .Include(r => r.Grades)
                    .ThenInclude(g => g.DisciplineClass)
                        .ThenInclude(dc => dc!.Teacher)

                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task InsertAsync(Registration reg)
        {
            bool alreadyExists = await _context.Registrations
                .AnyAsync(r =>
                    r.StudentId == reg.StudentId &&
                    r.StudentClassId == reg.StudentClassId);

            if (alreadyExists)
            {
                throw new IntegrityException(
                    "This student is already registered in this class.");
            }

            reg.CreatedAt = DateTime.UtcNow;

            _context.Registrations.Add(reg);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Registration reg)
        {
            bool exists = await _context.Registrations.AnyAsync(r => r.Id == reg.Id);

            if (!exists)
            {
                throw new NotFoundException("Registration Id not found.");
            }

            try
            {
                reg.UpdatedAt = DateTime.UtcNow;
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
                    throw new NotFoundException("Registration Id not found.");
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
