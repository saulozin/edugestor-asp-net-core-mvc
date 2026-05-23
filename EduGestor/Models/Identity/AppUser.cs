using Microsoft.AspNetCore.Identity;

namespace EduGestor.Models.Identity
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
