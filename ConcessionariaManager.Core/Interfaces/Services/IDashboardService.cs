using ConcessionariaManager.Core.Models.Dashboard;

namespace ConcessionariaManager.Core.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<VendaRelatorioViewModel> GerarRelatorioAsync(DateTime dataInicio, DateTime dataFim);
        Task<List<RelatorioVendaItemViewModel>> ExportarRelatorio(DateTime dataInicio, DateTime dataFim);
    }
}
