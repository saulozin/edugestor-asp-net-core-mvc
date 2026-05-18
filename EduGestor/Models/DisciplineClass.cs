using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduGestor.Models
{
    public class DisciplineClass
    {
        //Disciplina Turma

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid StudentClassId { get; set; }
        public StudentClass? StudentClass { get; set; }

        public Guid DisciplineId { get; set; }
        public Discipline? Discipline { get; set; }

        public Guid TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        public DisciplineClass()
        {
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
