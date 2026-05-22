using EduGestor.Models.Enums;

namespace EduGestor.Models.ViewModels
{
    public class StudentClassSearchViewModel
    {
        public string? ClassCode { get; set; }

        public EducationLevel? EduLevel { get; set; }

        public Series? StudentSeries { get; set; }

        public Shift? ClassShift { get; set; }

        public StudentClass? StudentClass { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public int TotalPages { get; set; }

        public IEnumerable<StudentClass?> StudentClasses { get; set; } = new List<StudentClass?>();
    }
}
