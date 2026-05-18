using System.ComponentModel.DataAnnotations;

namespace EduGestor.Models.Enums
{
    public enum EducationLevel : int
    {
        [Display(Name = "Early Childhood Education")]
        EarlyChildhoodEducation = 0, //Educação Infantil

        [Display(Name = "Elementary Education")]
        ElementaryEducation = 1, //Ensino Fundamental

        [Display(Name = "High School")]
        HighSchool = 2 //Ensino Médio
    }
}
