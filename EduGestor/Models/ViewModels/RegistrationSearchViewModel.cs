using EduGestor.Models.Enums;

namespace EduGestor.Models.ViewModels
{
    public class RegistrationSearchViewModel
    {
        public string? StudentName { get; set; }

        public RegistrationStatus? Status { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public Guid? StudentClassId { get; set; }

        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; }

        public int TotalItems { get; set; }

        public int PageSize { get; set; } = 5;

        public IEnumerable<Registration>? Registrations { get; set; }
    }
}
