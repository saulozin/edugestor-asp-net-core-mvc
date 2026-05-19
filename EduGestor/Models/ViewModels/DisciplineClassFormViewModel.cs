using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduGestor.Models.ViewModels
{
    public class DisciplineClassFormViewModel
    {
        public DisciplineClass? DisciplineClass { get; set; }

        public ICollection<SelectListItem> StudentClasses { get; set; }
            = new List<SelectListItem>();

        public ICollection<SelectListItem> Disciplines { get; set; }
            = new List<SelectListItem>();

        public ICollection<SelectListItem> Teachers { get; set; }
            = new List<SelectListItem>();
    }
}
