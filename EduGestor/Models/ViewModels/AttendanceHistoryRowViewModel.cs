namespace EduGestor.Models.ViewModels
{
    public class AttendanceHistoryRowViewModel
    {
        public string StudentName { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public bool Present { get; set; }
    }
}
