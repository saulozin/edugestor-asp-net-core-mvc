using EduGestor.Models;

namespace EduGestor.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }

        public int TotalGuardians { get; set; }

        public int TotalTeachers { get; set; }

        public int TotalRegistrations { get; set; }

        public int TotalStudentClasses { get; set; }

        public int SchoolYear { get; set; } = DateTime.Now.Year;

        public ICollection<Student> RecentStudents { get; set; } = new List<Student>();

        public ICollection<Registration> RecentRegistrations { get; set; } = new List<Registration>();
    }
}
