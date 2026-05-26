namespace EduGestor.ViewModels
{
    public class ReportCardViewModel
    {
        public string StudentName { get; set; } = string.Empty;

        public string GuardianName { get; set; } = string.Empty;

        public string ClassName { get; set; } = string.Empty;

        public int SchoolYear { get; set; }

        public decimal AverageGrade { get; set; }

        public decimal AverageFrequency { get; set; }

        public bool Approved { get; set; }

        public ICollection<ReportCardDisciplineViewModel> Disciplines { get; set; } 
            = new List<ReportCardDisciplineViewModel>();
    }

    public class ReportCardDisciplineViewModel
    {
        public string Discipline { get; set; } = string.Empty;

        public decimal Average { get; set; }

        public decimal Frequency { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
