using System.ComponentModel.DataAnnotations;

namespace EduGestor.Attributes
{
    public class CpfValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("CPF is required.");
            }

            var cpf = value.ToString()!
                .Replace(".", "")
                .Replace("-", "")
                .Trim();

            if (cpf.Length != 11)
            {
                return new ValidationResult("Invalid CPF.");
            }

            // Evita CPFs repetidos
            if (cpf.All(c => c == cpf[0]))
            {
                return new ValidationResult("Invalid CPF.");
            }

            // =========================
            // FIRST DIGIT
            // =========================

            int[] mult1 =
            {
                10, 9, 8, 7, 6, 5, 4, 3, 2
            };

            var sum = 0;

            for (int i = 0; i < 9; i++)
            {
                sum += (cpf[i] - '0') * mult1[i];
            }

            var remainder = sum % 11;

            var digit1 = remainder < 2 ? 0 : 11 - remainder;

            // =========================
            // SECOND DIGIT
            // =========================

            int[] mult2 =
            {
                11, 10, 9, 8, 7, 6, 5, 4, 3, 2
            };

            sum = 0;

            for (int i = 0; i < 10; i++)
            {
                sum += (cpf[i] - '0') * mult2[i];
            }

            remainder = sum % 11;

            var digit2 = remainder < 2 ? 0 : 11 - remainder;

            // =========================
            // VALIDATE
            // =========================

            if (cpf[9] - '0' != digit1 ||
                cpf[10] - '0' != digit2)
            {
                return new ValidationResult("Invalid CPF.");
            }

            return ValidationResult.Success;
        }
    }
}
