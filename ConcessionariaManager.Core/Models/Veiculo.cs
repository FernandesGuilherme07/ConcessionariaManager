using ConcessionariaManager.Core.Enums;
using ConcessionariaManager.Core.Utils.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcessionariaManager.Core.Models
{
    public sealed class Veiculo : ModelBase
    {
        [Required(ErrorMessage = "O modelo do veículo é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O modelo deve ter no máximo 100 caracteres.")]
        public string Modelo { get; set; } = string.Empty;

        [Display(Name = "Ano de fabricação")]
        [Required(ErrorMessage = "O ano de fabricação é obrigatório.")]
        [AnoFabricacao(ErrorMessage = "O ano de fabricação não pode ser maior que o ano atual.")]
        public int AnoFabricacao { get; set; }

        [Required(ErrorMessage = "A placa do veículo é obrigatória.")]
        public string Placa { get; set; } = string.Empty;

        public decimal Preco { get; set; }

        [NotMapped]
        [Display(Name = "Preço")]
        [RegularExpression(@"^\d{1,3}(\.\d{3})*(,\d{2})?$", ErrorMessage = "O campo Preço deve ser um número válido no formato 'R$ 1.000,00'")]
        [Required(ErrorMessage = "O preço do veículo é obrigatório.")]
        public string PrecoStringView { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de veículo é obrigatório.")]

        public TipoVeiculo TipoVeiculo { get; set; }

        [Required(ErrorMessage = "O fabricante é obrigatório.")]
        [Display(Name = "Fabricante")]
        public int FabricanteId { get; set; }

        public Fabricante? Fabricante { get; set; }

        [Required(ErrorMessage = "O fabricante é obrigatório.")]
        [Display(Name = "Fabricante")]
        public int ConcessionariaId { get; set; }

        public Concessionaria? Concessionaria { get; set; }

        public string? Descricao { get; set; }

        public bool Vendido { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
    }
}
