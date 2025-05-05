using Moq;
using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Core.Services;

namespace ConcessionariaManager.Tests.UnitTests.Venda
{

    public class VendaServiceTests
    {
        [Fact]
        public async Task CriarVendaAsync_DeveRetornarErro_SeVeiculoNaoExistir()
        {
            var builder = new VendaServiceBuilder();
            builder._repositoryMock.Setup(r => r.ObterVeiculoPorIdAsync(It.IsAny<int>()))
                                       .ReturnsAsync((Veiculo?)null);

            var service = builder.Build();
            var venda = new Core.Models.Venda { VeiculoId = 1, CPFCliente = "123.456.789-00", PrecoVenda = 50000 };

            var (sucesso, erro, _) = await service.CriarVendaAsync(venda);

            Assert.False(sucesso);
            Assert.Equal("Veículo não encontrado.", erro);
        }

        [Fact]
        public async Task CriarVendaAsync_DeveRetornarErro_SeVeiculoJaVendido()
        {
            var veiculo = new Veiculo { Id = 1, Vendido = true, Preco = 50000 };
            var builder = new VendaServiceBuilder();
            builder._repositoryMock.Setup(r => r.ObterVeiculoPorIdAsync(1))
                                       .ReturnsAsync(veiculo);

            var service = builder.Build();
            var venda = new Core.Models.Venda { VeiculoId = 1, CPFCliente = "123.456.789-00", PrecoVenda = 50000 };

            var (sucesso, erro, _) = await service.CriarVendaAsync(venda);

            Assert.False(sucesso);
            Assert.Equal("Veículo já vendido.", erro);
        }

        [Fact]
        public async Task CriarVendaAsync_DeveRetornarErro_SePrecoVendaMaiorQuePrecoVeiculo()
        {
            var veiculo = new Veiculo { Id = 1, Vendido = false, Preco = 50000 };
            var builder = new VendaServiceBuilder();
            builder._repositoryMock.Setup(r => r.ObterVeiculoPorIdAsync(1))
                                       .ReturnsAsync(veiculo);

            var service = builder.Build();
            var venda = new Core.Models.Venda { VeiculoId = 1, CPFCliente = "123.456.789-00", PrecoVenda = 60000 };

            var (sucesso, erro, _) = await service.CriarVendaAsync(venda);

            Assert.False(sucesso);
            Assert.Equal("O preço da venda não pode ser maior que o preço do veículo (R$ 50.000,00).", erro);
        }

        [Fact]
        public async Task CancelarVendaAsync_DeveRetornarErro_SeVendaNaoExistir()
        {
            var builder = new VendaServiceBuilder();
            builder._repositoryMock.Setup(r => r.ObterVendaPorIdAsync(It.IsAny<int>()))
                                       .ReturnsAsync((Core.Models.Venda?)null);

            var service = builder.Build();

            var (sucesso, erro) = await service.CancelarVendaAsync(1, "Motivo");

            Assert.False(sucesso);
            Assert.Equal("Venda inválida ou já cancelada.", erro);
        }

        [Fact]
        public async Task CancelarVendaAsync_DeveRetornarErro_SeVendaJaCancelada()
        {
            var venda = new Core.Models.Venda { Id = 1, Cancelada = true };
            var builder = new VendaServiceBuilder();
            builder._repositoryMock.Setup(r => r.ObterVendaPorIdAsync(1))
                                       .ReturnsAsync(venda);

            var service = builder.Build();

            var (sucesso, erro) = await service.CancelarVendaAsync(1, "Motivo");

            Assert.False(sucesso);
            Assert.Equal("Venda inválida ou já cancelada.", erro);
        }

        [Fact]
        public async Task CancelarVendaAsync_DeveRetornarErro_SeDataVendaPassouDe3Meses()
        {
            var venda = new Core.Models.Venda
            {
                Id = 1,
                Cancelada = false,
                DataVenda = DateTime.Now.AddMonths(-4),
                Veiculo = new Veiculo()
            };
            var builder = new VendaServiceBuilder();
            builder._repositoryMock.Setup(r => r.ObterVendaPorIdAsync(1))
                                       .ReturnsAsync(venda);

            var service = builder.Build();

            var (sucesso, erro) = await service.CancelarVendaAsync(1, "Fora do prazo");

            Assert.False(sucesso);
            Assert.Equal("A venda não pode ser cancelada após 3 meses da data da venda.", erro);
        }

        [Fact]
        public async Task CancelarVendaAsync_DeveCancelarVenda_SeValido()
        {
            var veiculo = new Veiculo { Vendido = true };
            var venda = new Core.Models.Venda
            {
                Id = 1,
                Cancelada = false,
                DataVenda = DateTime.Now.AddMonths(-2),
                Veiculo = veiculo
            };

            var builder = new VendaServiceBuilder();
            builder._repositoryMock.Setup(r => r.ObterVendaPorIdAsync(1))
                                       .ReturnsAsync(venda);

            var service = builder.Build();

            var (sucesso, erro) = await service.CancelarVendaAsync(1, "Cliente desistiu");

            Assert.True(sucesso);
            Assert.Null(erro);
            Assert.True(venda.Cancelada);
            Assert.Equal("Cliente desistiu", venda.MotivoDoCancelameto);
            Assert.False(venda.Veiculo!.Vendido);

            builder._repositoryMock.Verify(r => r.CancelarVendaAsync(venda), Times.Once);
        }

        [Fact]
        public async Task ObterVeiculosDisponiveisJsonAsync_DeveRetornarListaComIdEModello()
        {
            var veiculos = new List<Veiculo>
            {
                new Veiculo { Id = 1, Modelo = "Civic" },
                new Veiculo { Id = 2, Modelo = "Corolla" }
            };

            var service = new VendaServiceBuilder()
                .ComVeiculosDisponiveis(veiculos)
                .Build();

            var resultado = await service.ObterVeiculosDisponiveisJsonAsync(10);

            Assert.Equal(2, resultado.Count);
            Assert.Contains(resultado, v => v!.ToString()!.Contains("Civic"));
            Assert.Contains(resultado, v => v!.ToString()!.Contains("Corolla"));
        }

    }

}
