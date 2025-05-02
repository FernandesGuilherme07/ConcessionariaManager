using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ConcessionariaManager.Core.Utils.Validations
{
    public class CpfValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            var cpf = value.ToString()?.Replace(".", "").Replace("-", "").Trim();

            if (cpf is null || new string(cpf[0], cpf.Length) == cpf || !Regex.IsMatch(cpf, @"^\d{11}$"))
                return false;

            var numbers = cpf.Select(c => int.Parse(c.ToString())).ToArray();

            var sum = 0;
            for (int i = 0; i < 9; i++)
                sum += numbers[i] * (10 - i);

            var remainder = sum % 11;
            var firstVerifier = remainder < 2 ? 0 : 11 - remainder;

            if (numbers[9] != firstVerifier)
                return false;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += numbers[i] * (11 - i);

            remainder = sum % 11;
            var secondVerifier = remainder < 2 ? 0 : 11 - remainder;

            if (numbers[10] != secondVerifier)
                return false;

            return true;
        }
    }
}
