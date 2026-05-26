namespace EduGestor.Models.ViewModels
{
    public class TeacherPortalViewModel
    {
        public string? TeacherName { get; set; }

        public ICollection<TeacherClassViewModel> Classes { get; set; }
            = new List<TeacherClassViewModel>();
    }

}
