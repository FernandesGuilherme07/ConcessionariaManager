using System.ComponentModel.DataAnnotations;

namespace ConcessionariaManager.Core.Utils.Validations
{
    public class AnoFabricacaoAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int anoFabricacao)
            {
                var anoAtual = DateTime.Now.Year;
                if (anoFabricacao > anoAtual)
                {
                    return new ValidationResult($"O ano de fabricação não pode ser maior que o ano atual ({anoAtual}).");
                }
                if (anoFabricacao < 1950)
                {
                    return new ValidationResult("O ano de fabricação não pode ser anterior a 1950.");
                }
            }

            return ValidationResult.Success!;
        }
    }
}
