using EduGestor.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;

namespace EduGestor.ViewModels
{
    public class StudentFormViewModel
    {
        public Student? Student { get; set; }

        // dropdown
        public Guid? SelectedGuardianId { get; set; }
        public ICollection<SelectListItem> Guardians { get; set; } = new List<SelectListItem>();

        // criação inline
        public bool CreateNewGuardian { get; set; }
        public Guardian? NewGuardian { get; set; }

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}
