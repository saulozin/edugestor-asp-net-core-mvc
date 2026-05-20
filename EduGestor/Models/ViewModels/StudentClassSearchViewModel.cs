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

        public IEnumerable<StudentClass?> StudentClasses { get; set; } = new List<StudentClass?>();
    }
}
