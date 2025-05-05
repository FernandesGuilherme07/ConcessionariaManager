using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Core.Interfaces.Services;
using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Core.Services;
using Moq;

namespace ConcessionariaManager.Tests.UnitTests.Dashboard
{
    public class DashboardServiceBuilder
    {
        public Mock<IDashboardRepository> DashboardRepositoryMock { get; private set; } = new();
        public Mock<ICacheService> CacheServiceMock { get; private set; } = new();

        public DashboardServiceBuilder WithCacheReturning<T>(string key, T value) where T : class
        {
            CacheServiceMock.Setup(c => c.GetAsync<T>(key))
                            .ReturnsAsync(value);
            return this;
        }

        public DashboardServiceBuilder WithExportacaoRetornando(List<Core.Models.Dashboard.RelatorioVendaItemViewModel> dados, DateTime dataInicio, DateTime dataFim)
        {
            DashboardRepositoryMock.Setup(r => r.ObterDadosParaExportacao(dataInicio, dataFim))
                                    .ReturnsAsync(dados);
            return this;
        }

        public DashboardServiceBuilder WithVendasRetornando(List<Core.Models.Venda> vendas, DateTime dataInicio, DateTime dataFim)
        {
            DashboardRepositoryMock.Setup(r => r.ObterVendasAsync(dataInicio, dataFim))
                                    .ReturnsAsync(vendas);
            return this;
        }

        public DashboardService Build()
        {
            return new DashboardService(DashboardRepositoryMock.Object, CacheServiceMock.Object);
        }
    }

}
