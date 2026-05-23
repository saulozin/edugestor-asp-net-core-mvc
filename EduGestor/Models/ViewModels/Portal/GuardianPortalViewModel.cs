using EduGestor.Models;

namespace EduGestor.Models.ViewModels.Portal
{
    public class GuardianPortalViewModel
    {
        public Guardian? Guardian { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

    }
}
