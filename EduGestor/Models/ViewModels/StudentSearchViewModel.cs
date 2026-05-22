using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduGestor.Models.ViewModels
{
    public class StudentSearchViewModel
    {
        public string? SearchTerm { get; set; }

        public Guid? StudentClassId { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public int TotalPages { get; set; }

        public IEnumerable<SelectListItem> Classes { get; set; } = new List<SelectListItem>();

        public IEnumerable<Student>? Students { get; set; } = new List<Student>();
    }
}
