namespace EduGestor.Models.ViewModels
{
    public class TeacherClassViewModel
    {
        public Guid DisciplineClassId { get; set; }

        public Guid StudentClassId { get; set; }

        public Guid TeacherId { get; set; }

        public string Discipline { get; set; } = string.Empty;

        public string ClassCode { get; set; } = string.Empty;

        public string TeacherName { get; set; } = string.Empty;

        public int StudentsCount { get; set; }

        public decimal AverageGrade { get; set; }

        public decimal AverageFrequency { get; set; }

    }
}
