namespace EduGestor.Models.ViewModels
{
    public class GradeLaunchRowViewModel
    {
        public Guid RegistrationId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public decimal Grade { get; set; }

        public decimal Frequency { get; set; }
    }
}
