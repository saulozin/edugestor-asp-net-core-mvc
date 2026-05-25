using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduGestor.Models
{
    // Nota
    public class Grade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [Range(0.0, 10.0, ErrorMessage = "{0} must be from {1} to {2}")]
        public decimal StudentGrade { get; set; }   // Nota do estudante

        [Required(ErrorMessage = "{0} required")]
        [Range(0.0, 100.0, ErrorMessage = "{0} must be from {1} to {2}")]
        public decimal Frequency { get; set; }   // Frequencia

        [Required(ErrorMessage = "{0} required")]
        [Range(1, 4, ErrorMessage = "{0} must be from {1} to {2}")]
        public int Bimester { get; set; }    //Bimestre

        [Required(ErrorMessage = "{0} required")]
        public int SchoolYear { get; set; }     // Ano escolar

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Matrícula do aluno
        [Display(Name = "Registration")]
        public Guid? RegistrationId { get; set; }
        public Registration? Registration { get; set; }

        // Disciplina + turma + professor
        public Guid? DisciplineClassId { get; set; }
        public DisciplineClass? DisciplineClass { get; set; }

        public Grade()
        {
        }

    }
}
