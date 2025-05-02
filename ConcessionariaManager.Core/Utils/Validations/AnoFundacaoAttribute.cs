using System.ComponentModel.DataAnnotations;

namespace ConcessionariaManager.Core.Utils.Validations
{
    public class AnoFundacaoAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int anoFundacao)
            {
                var anoAtual = DateTime.Now.Year;
                if (anoFundacao > anoAtual)
                {
                    return new ValidationResult($"O ano de fundação não pode ser maior que o ano atual({anoAtual}).");
                }
                if (anoFundacao < 1600)
                {
                    return new ValidationResult("O ano de fundação não pode ser anterior a 1600.");
                }
            }

            return ValidationResult.Success!;
        }
    }

}
