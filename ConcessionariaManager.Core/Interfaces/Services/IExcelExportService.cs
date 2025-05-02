using ConcessionariaManager.Core.Models.Dashboard;

namespace ConcessionariaManager.Web.Core.Interfaces.Services
{
    public interface IExcelExportService
    {
        byte[] ExportarRelatorio(List<RelatorioVendaItemViewModel> relatorio);
    }
}
