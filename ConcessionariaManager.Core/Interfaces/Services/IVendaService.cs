using ConcessionariaManager.Core.Models;
using System.Collections;

namespace ConcessionariaManager.Core.Interfaces.Services
{
    public interface IVendaService
    {
        Task<IPagedList<Venda>> ObterVendasPaginadasAsync(string? searchString, int pageNumber, int pageSize);
        Task<List<Veiculo>> ObterVeiculosPorConcessionariaAsync(int concessionariaId);
        Task<Venda?> ObterDetalhesVendaAsync(int id);
        Task<(bool sucesso, string? erro)> CriarVendaAsync(Venda venda);
        Task<(bool sucesso, string? erro)> CancelarVendaAsync(int id, string motivoCancelamento);
        Task<IEnumerable<Concessionaria>> ObterConcessionariasAsync();
    }
}

