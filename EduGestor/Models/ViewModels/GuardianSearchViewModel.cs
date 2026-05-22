namespace EduGestor.Models.ViewModels
{
    public class GuardianSearchViewModel
    {
        public string? SearchTerm { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }

        public IEnumerable<Guardian>? Guardians { get; set; } = new List<Guardian>();
    }
}
