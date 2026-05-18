using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduGestor.Models
{
    //Estudante
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "{0} size should be between {2} and {1}")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [StringLength(8, ErrorMessage = "{0} size should be {1}")]
        public string Rg {  get; set; }

        [Required]
        [StringLength(11, ErrorMessage = "{0} size should be {1}")]
        public string Cpf { get; set; }

        [Required]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateOnly BirthDate { get; set; }

        [Display(Name="Photo")]
        public string? FotoUrl { get; set; }

        [Display(Name = "Guardian")]
        public Guid? GuardianId { get; set; }
        public Guardian? Guardian { get; set; }

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        public Student()
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

        public void AddGrade(Grade grade)
        {
            Grades.Add(grade);
        }

        public void RemoveGrade(Grade grade)
        {
            Grades.Remove(grade);
        }
    }
}
