using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduGestor.Models.ViewModels
{
    public class GradeFormViewModel
    {
        public Grade? Grade { get; set; }

        public ICollection<SelectListItem> Registrations { get; set; }
            = new List<SelectListItem>();

        public ICollection<SelectListItem> DisciplineClasses { get; set; }
            = new List<SelectListItem>();
    }
}
