using ConcessionariaManager.Core.Models;

namespace ConcessionariaManager.Core.Interfaces.Repositories
{
    public interface IVendaRepository
    {
        IQueryable<Venda> ObterVendas(string busca);
        IQueryable<Venda> ObterVendas();
        Task CancelarVendaAsync(Venda venda);
        IQueryable<Veiculo> ObterVeiculosList();
        IQueryable<Concessionaria> ObterConcessionariasList();
        Task<Venda?> ObterVendaPorIdAsync(int id);
        Task<List<Veiculo>> ObterVeiculosDisponiveisPorConcessionariaAsync(int concessionariaId);
        Task<Veiculo?> ObterVeiculoPorIdAsync(int id);
        Task AdicionarVendaAsync(Venda venda);
        Task RealizarVendaAsync(Venda venda);
        Task AtualizarVendaAsync(Venda venda);
        Task AtualizarVeiculoAsync(Veiculo veiculo);
        Task SaveChangesAsync();
        Task<IDisposable> BeginTransactionAsync();
    }
}
