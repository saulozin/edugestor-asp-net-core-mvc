namespace EduGestor.Models.ViewModels
{
    public class PagedViewModel<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public string? SearchTerm { get; set; }

        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; }

        public int TotalItems { get; set; }

        public int PageSize { get; set; } = 5;

        public bool HasPreviousPage =>
            PageNumber > 1;

        public bool HasNextPage =>
            PageNumber < TotalPages;
    }
}
