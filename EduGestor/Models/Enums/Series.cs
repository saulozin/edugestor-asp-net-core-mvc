using System.ComponentModel.DataAnnotations;

namespace EduGestor.Models.Enums
{
    public enum Series : int
    {
        //Ensino Infantil
        Daycare = 0,        //Creche
        Preschool = 1,      //Pré-escolar

        //Ensino Fundamental
        [Display(Name = "First Grade")]
        FirstGrade = 2, // -> Primeiro ano

        [Display(Name = "Second Grade")]
        SecondGrade = 3, // -> Segundo ano

        [Display(Name = "Third Grade")]
        ThirdGrade = 4, // -> Terceiro ano

        [Display(Name = "Fourth Grade")]
        FourthGrade = 5, // -> Quarto ano

        [Display(Name = "Fifth Grade")]
        FifthGrade = 6, // -> Quinto ano

        [Display(Name = "Sixth Grade")]
        SixthGrade = 7, // -> Sexto ano

        [Display(Name = "Seventh Grade")]
        SeventhGrade = 8, // -> Sétimo ano

        [Display(Name = "Eighth Grade")]
        EighthGrade = 9, // -> Oitavo ano

        [Display(Name = "Ninth Grade")]
        NinthGrade = 10, // -> Nono ano

        //Ensino Médio
        [Display(Name = "First Serie")]
        FirstSerie = 11, // -> Primeira Série do Ensino Médio

        [Display(Name = "Second Serie")]
        SecondSerie = 12, // -> Segunda Série do Ensino Médio

        [Display(Name = "Third Serie")]
        ThirdSerie = 13 // -> Terceira Série do Ensino Médio
    }
}
