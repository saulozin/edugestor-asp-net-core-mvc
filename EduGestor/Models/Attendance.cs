using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduGestor.Models
{
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateOnly Date { get; set; }

        [Required]
        public bool Present { get; set; }

        // =========================================
        // REGISTRATION
        // =========================================

        [Required]
        public Guid RegistrationId { get; set; }

        public Registration? Registration { get; set; }

        // =========================================
        // DISCIPLINE CLASS
        // =========================================

        [Required]
        public Guid DisciplineClassId { get; set; }

        public DisciplineClass? DisciplineClass { get; set; }

        // =========================================
        // OPTIONAL OBSERVATION
        // =========================================

        [StringLength(500)]
        public string? Observation { get; set; }

        // =========================================
        // AUDIT
        // =========================================

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
