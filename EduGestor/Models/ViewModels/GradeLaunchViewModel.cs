namespace EduGestor.Models.ViewModels
{
    public class GradeLaunchViewModel
    {
        public Guid DisciplineClassId { get; set; }

        public string Discipline { get; set; } = string.Empty;

        public string ClassCode { get; set; } = string.Empty;

        public int Bimester { get; set; }

        public List<GradeLaunchRowViewModel> Students { get; set; }
            = new List<GradeLaunchRowViewModel>();
    }
}
