namespace EduGestor.Models.ViewModels.Portal
{
    public class PortalViewModel
    {
        public string GuardianName { get; set; } = string.Empty;

        public ICollection<PortalStudentViewModel> Students { get; set; } = new List<PortalStudentViewModel>();
    }
}
