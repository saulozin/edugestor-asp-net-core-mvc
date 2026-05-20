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

        // Resultado
        public List<Grade> Grades { get; set; }
            = new();
    }
}
