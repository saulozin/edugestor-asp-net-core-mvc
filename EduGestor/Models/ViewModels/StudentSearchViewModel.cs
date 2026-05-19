using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduGestor.Models.ViewModels
{
    public class StudentSearchViewModel
    {
        public string? SearchTerm { get; set; }

        public Guid? StudentClassId { get; set; }

        public List<SelectListItem> Classes { get; set; }
            = new();

        public IEnumerable<Student> Students { get; set; }
            = new List<Student>();
    }
}
