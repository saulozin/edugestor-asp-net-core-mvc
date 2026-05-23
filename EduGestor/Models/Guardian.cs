using EduGestor.Attributes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduGestor.Models
{
    //Responsavel
    public class Guardian
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "{0} size should be between {2} and {1}")]
        public string Name { get; set; }

        [Required]
        [CpfValidation]
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

        public ICollection<Student> Students { get; set; } = new List<Student>();

        public Guardian()
        {
        }

        public void AddStudent(Student student)
        {
            Students.Add(student);
        }

        public void RemoveStudent(Student student)
        {
            Students.Remove(student);
        }
    }
}
