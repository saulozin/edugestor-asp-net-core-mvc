namespace EduGestor.Models.ViewModels
{
    public class DisciplineClassSearchViewModel
    {
        // Busca global
        public string? Search { get; set; }

        public string? ClassCode { get; set; }

        public string? DisciplineName { get; set; }

        public string? TeacherName { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public int TotalPages { get; set; }

        public IEnumerable<DisciplineClass> DisciplineClasses { get; set; } = new List<DisciplineClass>();
    }
}
