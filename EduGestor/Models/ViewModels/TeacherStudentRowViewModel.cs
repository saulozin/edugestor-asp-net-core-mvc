using EduGestor.Models.Enums;

namespace EduGestor.Models.ViewModels
{
    public class TeacherStudentRowViewModel
    {
        public Guid RegistrationId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public decimal AverageGrade { get; set; }

        public decimal Frequency { get; set; }

        public AcademicStatus Status { get; set; }
    }
}
