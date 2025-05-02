using ConcessionariaManager.Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace ConcessionariaManager.Core.Models
{
    public sealed class Concessionaria : ModelBase
    {
        [Display(Name = "Nome")]
        [MaxLength(100)]
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string? Nome { get; set; }

        [Display(Name = "Endereço")]
        public required Endereco Endereco { get; set; }

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "O campo Telefone é obrigatório.")]
        public string? Telefone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "O campo Email deve ser um endereço de email válido.")]
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        public required string Email { get; set; }

        [Display(Name = "Cpacidade Máxima de Veículos")]
        [Required(ErrorMessage = "O campo Capacidade Máxima de Veículos é obrigatório.")]
        [Range(1, 5000, ErrorMessage = "A capacidade máxima de veículos deve ser entre 1 e 1000.")]
        public int CapacidadeMaximaDeVeiculos { get; set; }
        public ICollection<Veiculo> Veiculos { get; set; } = [];
        public bool IsDeleted { get; set; } = false;

    }
}
