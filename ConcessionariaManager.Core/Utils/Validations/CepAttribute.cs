using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ConcessionariaManager.Core.Utils.Validations
{
    public class CepAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string cep && !string.IsNullOrWhiteSpace(cep))
            {
                var regex = new Regex(@"^\d{5}-?\d{3}$");
                if (!regex.IsMatch(cep))
                {
                    return new ValidationResult("O CEP informado é inválido. Utilize o formato 12345-678 ou 12345678.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
