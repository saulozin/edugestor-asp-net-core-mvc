using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduGestor.Controllers
{
    [Authorize]
    public class HomeRedirectController : Controller
    {
        public IActionResult Index()
        {
            // GUARDIAN
            if (User.IsInRole("Guardian"))
            {
                return RedirectToAction(
                    "Index",
                    "Portal");
            }

            // ADMIN / SECRETARY / TEACHER
            return RedirectToAction(
                "Index",
                "Home");
        }
    }
}
