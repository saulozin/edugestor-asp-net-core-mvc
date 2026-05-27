namespace EduGestor.Models.ViewModels
{
    public class AttendanceLaunchRowViewModel
    {
        public Guid RegistrationId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public bool Present { get; set; } = true;

        public string? Observation { get; set; }
    }
}
