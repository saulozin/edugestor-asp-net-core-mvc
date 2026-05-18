using EduGestor.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduGestor.Models
{
    //Turma
    public class StudentClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public EducationLevel Level { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public Series Series { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public Shift Shift { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [Range(10, 30, ErrorMessage = "{0} must be from {1} to {2}")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [Display(Name = "Class Code")]
        public string Code { get; set; }

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<DisciplineClass> DisciplineClasses { get; set; } = new List<DisciplineClass>();

        public StudentClass()
        {
        }

        public void AddRegistration(Registration registration)
        {
            Registrations.Add(registration);
        }

        public void RemoveRegistration(Registration registration)
        {
            Registrations.Remove(registration);
        }

        public void AddDisciplineClass(DisciplineClass disciplineClass)
        {
            DisciplineClasses.Add(disciplineClass);
        }

        public void RemoveDisciplineClass(DisciplineClass disciplineClass)
        {
            DisciplineClasses.Remove(disciplineClass);
        }
    }
}
