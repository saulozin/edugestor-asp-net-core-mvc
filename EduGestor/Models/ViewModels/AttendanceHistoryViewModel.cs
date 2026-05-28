namespace EduGestor.Models.ViewModels
{
    public class AttendanceHistoryViewModel
    {
        public Guid DisciplineClassId { get; set; }

        public string Discipline { get; set; } = string.Empty;

        public string ClassCode { get; set; } = string.Empty;

        public List<AttendanceHistoryRowViewModel> Attendances { get; set; }
            = new();
    }
}
