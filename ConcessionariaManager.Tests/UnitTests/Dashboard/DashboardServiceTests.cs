using Moq;
using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Core.Interfaces.Services;
using ConcessionariaManager.Core.Models.Dashboard;
using ConcessionariaManager.Core.Services;

namespace ConcessionariaManager.Tests.UnitTests.Dashboard 
{

    public class DashboardServiceTests
    {
        [Fact]
        public async Task GerarRelatorioAsync_DeveRetornarDadosDoCache_SeExistirem()
        {
            var dataInicio = new DateTime(2025, 1, 1);
            var dataFim = new DateTime(2025, 1, 31);
            var cacheKey = $"dashboard:relatorio:{dataInicio:yyyyMMdd}:{dataFim:yyyyMMdd}";
            var viewModelEsperado = new VendaRelatorioViewModel { TotalVendas = 10 };


            var builder = new DashboardServiceBuilder()
                            .WithCacheReturning(cacheKey, viewModelEsperado);

            var service = builder.Build();


            var resultado = await service.GerarRelatorioAsync(dataInicio, dataFim);

            Assert.Equal(viewModelEsperado.TotalVendas, resultado.TotalVendas);
            builder.DashboardRepositoryMock
                .Verify(r => r.ObterVendasAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task ExportarRelatorio_DeveChamarRepositorioComParametrosCorretos()
        {
            
            var dataInicio = new DateTime(2025, 1, 1);
            var dataFim = new DateTime(2025, 1, 31);

            var builder = new DashboardServiceBuilder()
                .WithExportacaoRetornando(new List<RelatorioVendaItemViewModel>(), dataInicio, dataFim);

            var service = builder.Build();

            var resultado = await service.ExportarRelatorio(dataInicio, dataFim);

            Assert.NotNull(resultado);
            builder.DashboardRepositoryMock
                .Verify(r => r.ObterDadosParaExportacao(dataInicio, dataFim), Times.Once);
        }

        [Fact]
        public async Task GerarRelatorioAsync_DeveLancarExcecao_SeDataFimForMenorQueDataInicio()
        {
            var dataInicio = new DateTime(2025, 2, 1);
            var dataFim = new DateTime(2025, 1, 31);

            var service = new DashboardServiceBuilder().Build();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.GerarRelatorioAsync(dataInicio, dataFim));
        }

        [Fact]
        public async Task ExportarRelatorio_DeveLancarExcecao_SeDataFimForMenorQueDataInicio()
        {
            var dataInicio = new DateTime(2025, 2, 1);
            var dataFim = new DateTime(2025, 1, 31);

            var service = new DashboardServiceBuilder().Build();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.ExportarRelatorio(dataInicio, dataFim));
        }

    }

}
