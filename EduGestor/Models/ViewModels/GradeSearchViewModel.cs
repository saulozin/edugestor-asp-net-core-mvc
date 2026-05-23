namespace EduGestor.Models.ViewModels
{
    public class GradeSearchViewModel
    {
        // Busca global
        public string? Search { get; set; }

        // Nota
        public decimal? MinGrade { get; set; }
        public decimal? MaxGrade { get; set; }

        // Frequência
        public decimal? MinFrequency { get; set; }

        // Situação
        public bool? Approved { get; set; }

        // Acadêmico
        public int? Bimester { get; set; }
        public int? SchoolYear { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 8;

        public int TotalPages { get; set; }

        // Resultado
        public IEnumerable<Grade> Grades { get; set; } = new List<Grade>();
    }
}
