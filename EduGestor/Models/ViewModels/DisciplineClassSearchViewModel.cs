namespace EduGestor.Models.ViewModels
{
    public class DisciplineClassSearchViewModel
    {
        // Busca global
        public string? Search { get; set; }

        public string? ClassCode { get; set; }

        public string? DisciplineName { get; set; }

        public string? TeacherName { get; set; }

        public ICollection<DisciplineClass> DisciplineClasses { get; set; } = new List<DisciplineClass>();
    }
}
