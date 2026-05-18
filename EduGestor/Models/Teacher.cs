using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduGestor.Models
{
    //Professor
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "{0} size should be between {2} and {1}")]
        public string Name { get; set; }

        [Required]
        [StringLength(11, ErrorMessage = "{0} size should be {1}")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<DisciplineClass> DisciplineClasses { get; set; } = new List<DisciplineClass>();

        public Teacher()
        {
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
