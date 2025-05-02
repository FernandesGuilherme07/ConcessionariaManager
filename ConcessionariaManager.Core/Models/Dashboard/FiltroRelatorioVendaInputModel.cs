using System.ComponentModel.DataAnnotations;

namespace ConcessionariaManager.Core.Models.Dashboard
{
    public class FiltroRelatorioVendaInputModel
    {
        [Required]
        [Display(Name = "Mês")]
        [Range(1, 12)]
        public int Mes { get; set; }

        [Required]
        [Display(Name = "Ano")]
        public int Ano { get; set; }
    }
}
