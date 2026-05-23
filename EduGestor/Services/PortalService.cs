using EduGestor.Data;
using EduGestor.Models;
using EduGestor.Models.Identity;
using EduGestor.Models.ViewModels.Portal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduGestor.Services
{
    public class PortalService
    {
        private readonly EduGestorContext _context;

        private readonly UserManager<AppUser> _userManager;

        public PortalService(
            EduGestorContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;

            _userManager = userManager;
        }

        public async Task<GuardianPortalViewModel>
            GetGuardianPortalAsync(
                ClaimsPrincipal userPrincipal)
        {
            var userId =
                _userManager.GetUserId(userPrincipal);

            var guardian = await _context.Guardians
                .Include(g => g.Students)
                    .ThenInclude(s => s.Registrations)
                .FirstOrDefaultAsync(g =>
                    g.UserId == userId);

            if (guardian == null)
            {
                return new GuardianPortalViewModel();
            }

            var studentIds =
                guardian.Students
                    .Select(s => s.Id)
                    .ToList();

            var registrations =
                await _context.Registrations
                    .Include(r => r.StudentClass)
                    .Where(r =>
                        studentIds.Contains(r.StudentId))
                    .ToListAsync();

            var grades =
                await _context.Grades
                    .Include(g => g.Registration)
                        .ThenInclude(r => r!.Student)
                    .Include(g => g.DisciplineClass)
                        .ThenInclude(dc => dc!.Discipline)
                    .Where(g =>
                        studentIds.Contains(
                            g.Registration!.StudentId))
                    .ToListAsync();

            return new GuardianPortalViewModel
            {
                Guardian = guardian,

                Students = guardian.Students.ToList(),

                Registrations = registrations,

                Grades = grades
            };
        }
    }
}
