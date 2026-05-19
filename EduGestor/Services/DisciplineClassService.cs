using EduGestor.Data;
using EduGestor.Models;
using EduGestor.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EduGestor.Services
{
    public class DisciplineClassService
    {
        private readonly EduGestorContext _context;

        public DisciplineClassService(EduGestorContext context)
        {
            _context = context;
        }

        // =========================
        // FIND ALL
        // =========================
        public async Task<List<DisciplineClass>> FindAllAsync()
        {
            return await _context.DisciplineClasses
                .Include(dc => dc.StudentClass)
                .Include(dc => dc.Discipline)
                .Include(dc => dc.Teacher)
                .OrderBy(dc => dc.StudentClass!.Code)
                .ToListAsync();
        }

        // =========================
        // FIND BY ID
        // =========================
        public async Task<DisciplineClass?> FindByIdAsync(Guid id)
        {
            return await _context.DisciplineClasses
                .Include(dc => dc.StudentClass)
                .Include(dc => dc.Discipline)
                .Include(dc => dc.Teacher)
                .Include(dc => dc.Grades)
                .FirstOrDefaultAsync(dc => dc.Id == id);
        }

        // =========================
        // INSERT
        // =========================
        public async Task InsertAsync(DisciplineClass dc)
        {
            bool alreadyExists = await _context.DisciplineClasses
                .AnyAsync(x =>
                    x.StudentClassId == dc.StudentClassId &&
                    x.DisciplineId == dc.DisciplineId);

            if (alreadyExists)
            {
                throw new IntegrityException(
                    "This discipline is already linked to this class.");
            }

            _context.DisciplineClasses.Add(dc);

            await _context.SaveChangesAsync();
        }

        // =========================
        // UPDATE
        // =========================
        public async Task UpdateAsync(DisciplineClass dc)
        {
            bool exists = await _context.DisciplineClasses
                .AnyAsync(x => x.Id == dc.Id);

            if (!exists)
            {
                throw new NotFoundException(
                    "DisciplineClass Id not found.");
            }

            bool duplicated = await _context.DisciplineClasses
                .AnyAsync(x =>
                    x.Id != dc.Id &&
                    x.StudentClassId == dc.StudentClassId &&
                    x.DisciplineId == dc.DisciplineId);

            if (duplicated)
            {
                throw new IntegrityException(
                    "This discipline is already linked to this class.");
            }

            try
            {
                _context.DisciplineClasses.Update(dc);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException err)
            {
                throw new DbConcurrencyException(err.Message);
            }
        }

        // =========================
        // DELETE
        // =========================
        public async Task RemoveAsync(Guid id)
        {
            try
            {
                var dc = await _context.DisciplineClasses
                    .FindAsync(id);

                if (dc == null)
                {
                    throw new NotFoundException(
                        "DisciplineClass Id not found.");
                }

                _context.DisciplineClasses.Remove(dc);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException err)
            {
                throw new IntegrityException(
                    "Cannot delete because there are grades linked to this discipline class.");
            }
        }
    }
}
