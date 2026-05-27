using EduGestor.Models.Enums;

namespace EduGestor.Services
{
    public class AcademicRulesService
    {
        public AcademicStatus CalculateStatus(decimal average, decimal frequency)
        {
            if (frequency < 75)
            {
                return AcademicStatus.FailedByAttendance;
            }

            if (average >= 6)
            {
                return AcademicStatus.Approved;
            }

            if (average >= 4)
            {
                return AcademicStatus.Recovery;
            }

            return AcademicStatus.FailedByGrade;
        }
    }
}
