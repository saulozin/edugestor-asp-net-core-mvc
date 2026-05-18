using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using EduGestor.Data;

namespace EduGestor.Extensions
{
    public class ValidateExtensions
    {
        private readonly EduGestorContext _context;

        public ValidateExtensions(EduGestorContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync<T>(
            Expression<Func<T, bool>> predicate)
            where T : class
        {
            return await _context.Set<T>()
                .AnyAsync(predicate);
        }
    }
}
