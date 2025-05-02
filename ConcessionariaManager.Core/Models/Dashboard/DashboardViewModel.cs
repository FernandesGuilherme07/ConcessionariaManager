namespace ConcessionariaManager.Core.Models.Dashboard
{
    public class DashboardViewModel
    {
        public FiltroRelatorioVendaInputModel Filtro { get; set; } = new();
        public List<RelatorioVendaItemViewModel> Relatorio { get; set; } = [];

        public bool RelatorioGerado => Relatorio.Any();

        public string? Erro { get; set; }
    }

}
