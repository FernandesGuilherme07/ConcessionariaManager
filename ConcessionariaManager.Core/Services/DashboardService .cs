using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Core.Interfaces.Services;
using ConcessionariaManager.Core.Models.Dashboard;

namespace ConcessionariaManager.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;
        private readonly ICacheService _cacheService;
        private const string CachePrefix = "dashboard:relatorio";

        public DashboardService(IDashboardRepository repository, ICacheService cacheService)
        {
            _repository = repository;
            _cacheService = cacheService;
        }

        public async Task<List<RelatorioVendaItemViewModel>> ExportarRelatorio(DateTime dataInicio, DateTime dataFim)
        {

            return await _repository.ObterDadosParaExportacao(dataInicio, dataFim);
        }
        public async Task<VendaRelatorioViewModel> GerarRelatorioAsync(DateTime dataInicio, DateTime dataFim)
        {
            string cacheKey = $"{CachePrefix}:{dataInicio:yyyyMMdd}:{dataFim:yyyyMMdd}";

            var cache = await _cacheService.GetAsync<VendaRelatorioViewModel>(cacheKey);
            if (cache != null)
                return cache;

            var vendas = await _repository.ObterVendasAsync(dataInicio, dataFim);

            var viewModel = new VendaRelatorioViewModel
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                Periodo = $"{dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}",
                TotalVendas = vendas.Count,
                VendasPorTipoVeiculo = vendas
                    .GroupBy(v => v.Veiculo.TipoVeiculo.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),
                VendasPorFabricante = vendas
                    .GroupBy(v => v.Veiculo.Fabricante!.Nome)
                    .ToDictionary(g => g.Key, g => g.Count()),
                VendasPorConcessionaria = vendas
                    .GroupBy(v => v.Concessionaria!.Nome!)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            await _cacheService.SetAsync(cacheKey, viewModel, TimeSpan.FromMinutes(5));
            return viewModel;
        }
    }
}
