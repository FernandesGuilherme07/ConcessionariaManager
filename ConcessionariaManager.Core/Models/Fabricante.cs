using ConcessionariaManager.Core.Utils.Validations;
using System.ComponentModel.DataAnnotations;

namespace ConcessionariaManager.Core.Models
{
    public sealed class Fabricante : ModelBase
    {
        [Display(Name = "Nome")]
        [MaxLength(100)]
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(50)]
        [Display(Name = "País de Origem")]
        [Required(ErrorMessage = "O campo País de Origem é obrigatório.")]
        public string PaisOrigem { get; set; } = string.Empty;

        [Display(Name = "Ano de Fundação")]
        [Required(ErrorMessage = "O campo Ano de Fundação é obrigatório.")]
        [AnoFundacao(ErrorMessage = "O ano de fundação não pode ser maior que o ano atual.")]
        public int AnoFundacao { get; set; }

        [Display(Name = "Website URL")]
        [Url(ErrorMessage = "O Website deve ser uma URL válida (ex: https://www.exemplo.com).")]
        [Required(ErrorMessage = "O campo Website URL é obrigatório.")]
        public string Website { get; set; } = string.Empty;

        public ICollection<Veiculo>? Veiculos { get; set; } = [];
        public bool IsDeleted { get; set; } = false;
    }
}
