using System.ComponentModel.DataAnnotations;

namespace ConcessionariaManager.Core.Models
{
    public abstract class ModelBase
    {
        public int Id { get; set; }
        [Display(Name = "Data de criação")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Display(Name = "Ultima atualização")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
