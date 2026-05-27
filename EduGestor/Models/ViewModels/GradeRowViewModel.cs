using EduGestor.Models.Enums;

namespace EduGestor.Models.ViewModels
{
    public class GradeRowViewModel
    {
        public Guid Id { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string ClassCode { get; set; } = string.Empty;

        public string Discipline { get; set; } = string.Empty;

        public string TeacherName { get; set; } = string.Empty;

        public decimal Grade { get; set; }

        public decimal Frequency { get; set; }

        public AcademicStatus Status { get; set; }

        public int Bimester { get; set; }

        public int SchoolYear { get; set; }
    }
}
