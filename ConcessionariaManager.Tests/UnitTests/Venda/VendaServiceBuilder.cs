using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Core.Services;
using Moq;

namespace ConcessionariaManager.Tests.UnitTests.Venda
{
    public class VendaServiceBuilder
    {
        public readonly Mock<IVendaRepository> _repositoryMock = new();


        public VendaServiceBuilder ComVenda(Core.Models.Venda? venda)
        {
            _repositoryMock.Setup(r => r.ObterVendaPorIdAsync(It.IsAny<int>())).ReturnsAsync(venda);
            return this;
        }

        public VendaServiceBuilder ComCancelamentoSucesso()
        {
            _repositoryMock.Setup(r => r.CancelarVendaAsync(It.IsAny<Core.Models.Venda>())).Returns(Task.CompletedTask);
            return this;
        }

        public VendaServiceBuilder ComVeiculosDisponiveis(List<Veiculo> veiculos)
        {
            _repositoryMock.Setup(r => r.ObterVeiculosDisponiveisPorConcessionariaAsync(It.IsAny<int>()))
                           .ReturnsAsync(veiculos);
            return this;
        }

        public VendaService Build() => new(_repositoryMock.Object);

        public Mock<IVendaRepository> ObterMock() => _repositoryMock;
    }

}
