namespace EduGestor.Models.ViewModels.Portal
{
    public class PortalStudentViewModel
    {
        public Guid StudentId { get; set; }

        public Guid RegistrationId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string ClassCode { get; set; } = string.Empty;

        public string GuardianName { get; set; } = string.Empty;

        public ICollection<PortalGradeViewModel> Grades { get; set; } = new List<PortalGradeViewModel>();

    }
}
