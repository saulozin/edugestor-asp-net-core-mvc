namespace EduGestor.Models.ViewModels.Portal
{
    public class PortalGradeViewModel
    {
        public string Discipline { get; set; } = string.Empty;

        public decimal Grade { get; set; }

        public decimal Frequency { get; set; }

        public int Bimester { get; set; }

        public bool Approved { get; set; }
    }
}
