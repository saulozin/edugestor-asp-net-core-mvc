using System.Collections;

namespace EduGestor.Models.ViewModels
{
    public class RegistrationFormViewModel
    {
        public Registration? Registration { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}
