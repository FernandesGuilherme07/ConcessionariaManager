using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Core.Models.Dashboard;

namespace ConcessionariaManager.Core.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        Task<List<Venda>> ObterVendasAsync(DateTime dataInicio, DateTime dataFim);
        Task<List<RelatorioVendaItemViewModel>> ObterDadosParaExportacao(DateTime dataInicio, DateTime dataFim);
    }
}
