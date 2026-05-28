using EduGestor.Models.Enums;
using EduGestor.Models.ViewModels;

namespace EduGestor.ViewModels
{
    public class TeacherClassDetailsViewModel
    {
        public Guid DisciplineClassId { get; set; }

        public string Discipline { get; set; } = string.Empty;

        public string ClassCode { get; set; } = string.Empty;

        public string TeacherName { get; set; } = string.Empty;

        public string? Search { get; set; }

        public decimal? MaxFrequency { get; set; }

        public AcademicStatus? StatusFilter { get; set; }

        public string? SortBy { get; set; }

        public bool Descending { get; set; }

        public ICollection<TeacherStudentRowViewModel> Students { get; set; } 
            = new List<TeacherStudentRowViewModel>();
    }
}
