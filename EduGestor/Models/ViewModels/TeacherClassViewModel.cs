namespace EduGestor.Models.ViewModels
{
    public class TeacherClassViewModel
    {
        public Guid DisciplineClassId { get; set; }

        public string? Discipline { get; set; }

        public string? ClassCode { get; set; }

        public int StudentsCount { get; set; }

        public decimal AverageGrade { get; set; }

        public decimal AverageFrequency { get; set; }
    }
}
