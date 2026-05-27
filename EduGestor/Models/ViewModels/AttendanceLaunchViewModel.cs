namespace EduGestor.Models.ViewModels
{
    public class AttendanceLaunchViewModel
    {
        public Guid DisciplineClassId { get; set; }

        public string Discipline { get; set; }
            = string.Empty;

        public string ClassCode { get; set; }
            = string.Empty;

        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public List<AttendanceLaunchRowViewModel> Students { get; set; } = new();
    }
}
