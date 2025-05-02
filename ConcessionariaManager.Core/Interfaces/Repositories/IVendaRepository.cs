using ConcessionariaManager.Core.Models;

namespace ConcessionariaManager.Core.Interfaces.Repositories
{
    public interface IVendaRepository
    {
        IQueryable<Venda> ObterVendas();
        Task<IEnumerable<Concessionaria>> ObterConcessionariasAsync();
        Task<Venda?> ObterPorIdAsync(int id);
        Task<List<Veiculo>> ObterVeiculosPorConcessionariaAsync(int concessionariaId);
        Task AdicionarAsync(Venda venda);
        void Atualizar(Venda venda);
        Task SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

}
