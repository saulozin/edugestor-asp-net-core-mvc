using System.Text.RegularExpressions;

namespace EduGestor.Extensions
{
    public static class StringExtensions
    {
        public static string ToCpf(this string cpf)
        {
            return Convert
                .ToUInt64(cpf)
                .ToString(@"000\.000\.000\-00");
        }

        public static string ToRg(this string rg)
        {
            return Convert
                .ToUInt64(rg)
                .ToString(@"0000000\-0");
        }

        public static string ToPhone(this string phone)
        {
            return Regex.Replace(
                phone,
                @"(\d{2})(\d{5})(\d{4})",
                "($1)$2-$3");
        }

        public static string OnlyNumbers(this string value)
        {
            return Regex.Replace(value ?? "", @"\D", "");
        }
    }
}
