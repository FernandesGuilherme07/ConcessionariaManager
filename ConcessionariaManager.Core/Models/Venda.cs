using ConcessionariaManager.Core.Utils.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcessionariaManager.Core.Models
{
    public sealed class Venda : ModelBase
    {
        [Display(Name = "Conssecionária")]
        [Required(ErrorMessage = "A concessionária é obrigatória.")]
        public int ConcessionariaId { get; set; }
        public Concessionaria? Concessionaria { get; set; }

        [Display(Name = "Modelo do veículo")]
        [Required(ErrorMessage = "O veículo é obrigatório.")]
        public int VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }

        [Display(Name = "Nome do cliente")]
        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome do cliente deve ter no máximo 100 caracteres.")]
        public string NomeCliente { get; set; } = string.Empty;

        [Display(Name = "Cpf do cliente")]
        [Required(ErrorMessage = "O CPF do cliente é obrigatório.")]
        [CpfValidation(ErrorMessage = "O CPF informado é inválido.")]
        public string CPFCliente { get; set; } = string.Empty;

        [Display(Name = "Telefone do cliente")]
        [Required(ErrorMessage = "O telefone do cliente é obrigatório.")]
        public string? Telefone { get; set; }

        [Display(Name = "Data da venda")]
        [Required(ErrorMessage = "A data da venda é obrigatória.")]
        public DateTime DataVenda { get; set; }

        [Display(Name = "Preço da venda")]
        [Required(ErrorMessage = "O preço de venda é obrigatório.")]
        public decimal PrecoVenda { get; set; }

        [NotMapped]
        [Display(Name = "Preço da venda")]
        [RegularExpression(@"^\d{1,3}(\.\d{3})*(,\d{2})?$", ErrorMessage = "O campo Preço deve ser um número válido no formato 'R$ 1.000,00'")]
        [Required(ErrorMessage = "O preço do veículo é obrigatório.")]
        public string PrecoVendaStringView { get; set; } = string.Empty;

        public bool Cancelada { get; set; } = false;
        public string? MotivoDoCancelameto { get; set; }
        public string? ProtocoloVenda { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
