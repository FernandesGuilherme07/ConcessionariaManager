using ConcessionariaManager.Core.Utils.Validations;
using System.ComponentModel.DataAnnotations;

namespace ConcessionariaManager.Core.ValueObjects
{
    public class Endereco
    {
        [Display(Name = "Cep")]
        [Required(ErrorMessage = "O campo Cep é obrigatório.")]
        [Cep(ErrorMessage = "O campo Cep deve ser um CEP válido.")]
        public required string CEP { get; set; }

        [Display(Name = "Rua")]
        [Required(ErrorMessage = "O campo Rua é obrigatório.")]
        public required string Rua { get; set; }

        [Display(Name = "Número")]
        public string? Numero { get; set; }

        [Display(Name = "Complemento")]
        [MaxLength(50, ErrorMessage = "O campo Complemento deve ter no máximo 50 caracteres.")]
        public string? Complemento { get; set; }

        [Display(Name = "Bairro")]
        [Required(ErrorMessage = "O campo Bairro é obrigatório.")]
        public required string Bairro { get; set; }

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "O campo Cidade é obrigatório.")]
        public required string Cidade { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "O campo Estado é obrigatório.")]
        public required string Estado { get; set; }

        [Display(Name = "Endereço completo")]
        public string EnderecoCompleto
        {
            get
            {
                var endereco = $"{Rua}, {Numero ?? "s/n"}";

                if (!string.IsNullOrWhiteSpace(Complemento))
                    endereco += $" - {Complemento}";

                endereco += $", {Bairro}, {Cidade} - {Estado}, CEP: {CEP}";

                return endereco;
            }
            set { }
        }
    }

}
