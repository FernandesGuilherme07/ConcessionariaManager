namespace ConcessionariaManager.Core.Models.Dashboard
{
    public class VendaRelatorioViewModel
    {
        public bool RangeInvalido { get; set; } = false;
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? Periodo { get; set; }
        public int TotalVendas { get; set; }
        public Dictionary<string, int> VendasPorTipoVeiculo { get; set; } = new();
        public Dictionary<string, int> VendasPorFabricante { get; set; } = new();
        public Dictionary<string, int> VendasPorConcessionaria { get; set; } = new();

        public bool TemDados => TotalVendas > 0;
    }

}
