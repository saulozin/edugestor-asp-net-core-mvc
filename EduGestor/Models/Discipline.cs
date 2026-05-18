using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduGestor.Models
{
    public class Discipline
    {
        //Disciplina

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "{0} size should be between {2} and {1}")]
        public string Name { get; set; }

        [Required]
        public int Workload { get; set; }  //Carga Horária

        public ICollection<DisciplineClass> DisciplineClasses { get; set; } = new List<DisciplineClass>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        public Discipline()
        {
        }

        public void AddDisciplineClass(DisciplineClass classes)
        {
            DisciplineClasses.Add(classes);
        }

        public void RemoveDisciplineClass(DisciplineClass classes)
        {
            DisciplineClasses.Remove(classes);
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
