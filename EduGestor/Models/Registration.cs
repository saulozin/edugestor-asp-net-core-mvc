using EduGestor.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduGestor.Models
{
    //Matricula
    public class Registration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Registration Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateOnly Date {  get; set; }

        [Required]
        public RegistrationStatus Status { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }

        [Required(ErrorMessage = "Student Class is required")]
        [Display(Name = "Student Class")]
        public Guid StudentClassId { get; set; }
        public StudentClass? StudentClass { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        public Registration()
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

        public decimal GetAverage()
        {
            if (Grades == null || !Grades.Any())
            {
                return 0;
            }

            return Grades.Average(g => g.StudentGrade);
        }

        public decimal GetDisciplineAverage(Guid disciplineClassId)
        {
            var grades = Grades
                .Where(g => g.DisciplineClassId == disciplineClassId)
                .ToList();

            if (!grades.Any())
            {
                return 0;
            }

            return grades.Average(g => g.StudentGrade);
        }

        public decimal GetAttendance(Guid disciplineClassId)
        {
            var grades = Grades
                .Where(g => g.DisciplineClassId == disciplineClassId)
                .ToList();

            if (!grades.Any())
            {
                return 0;
            }

            return grades.Average(g => g.Frequency);
        }

        public bool IsApproved(Guid disciplineClassId)
        {
            var average = GetDisciplineAverage(disciplineClassId);

            var attendance = GetAttendance(disciplineClassId);

            return average >= 6.0m && attendance >= 75.0m;
        }

        public string GetStatus(Guid disciplineClassId)
        {
            return IsApproved(disciplineClassId)
                ? "Approved"
                : "Failed";
        }

    }
}
