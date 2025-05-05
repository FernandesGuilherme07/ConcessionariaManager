using ConcessionariaManager.Core.Interfaces.Services;
using ConcessionariaManager.Core.Models.Dashboard;
using ConcessionariaManager.Web.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConcessionariaManager.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IExcelExportService _excelExportService;

    public DashboardController(IDashboardService dashboardService, IExcelExportService excelExportService)
    {
        _dashboardService = dashboardService;
        _excelExportService = excelExportService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var dataFim = DateTime.Today;
        var dataInicio = dataFim.AddDays(-29);

        var viewModel = await _dashboardService.GerarRelatorioAsync(dataInicio, dataFim);

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> GerarRelatorio(DateTime dataInicio, DateTime dataFim)
    {
        if ((dataFim - dataInicio).TotalDays > 31)
        {
            return View("Index", new VendaRelatorioViewModel
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                Periodo = $"{dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}",
                RangeInvalido = true
            });
        }

        var viewModel = await _dashboardService.GerarRelatorioAsync(dataInicio, dataFim);

        return View("Index", viewModel);
    }

    [HttpPost]

    [Authorize(Roles = "Gerente,Administrador")]
    public async Task<IActionResult> ExportarExcel(DateTime dataInicio, DateTime dataFim)
    {
        var relatorioViewModel = await _dashboardService.ExportarRelatorio(dataInicio, dataFim);

        var arquivo = _excelExportService.ExportarRelatorio(relatorioViewModel);

        return File(arquivo,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"relatorio-vendas-{dataInicio:yyyy-MM-dd}_a_{dataFim:yyyy-MM-dd}.xlsx");
    }
}
