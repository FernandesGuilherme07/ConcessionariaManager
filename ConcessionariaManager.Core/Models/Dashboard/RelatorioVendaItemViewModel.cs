namespace ConcessionariaManager.Core.Models.Dashboard
{
    public class RelatorioVendaItemViewModel
    {
        public string TipoVeiculo { get; set; } = string.Empty;
        public string Fabricante { get; set; } = string.Empty;
        public string Concessionaria { get; set; } = string.Empty;
        public int QuantidadeVendida { get; set; }
        public decimal TotalVendido { get; set; }
    }


}
