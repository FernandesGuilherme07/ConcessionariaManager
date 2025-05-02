using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Core.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace ConcessionariaManager.Web.Data.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public VendaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Venda> ObterVendas() =>
            _context.Vendas
                .Include(v => v.Veiculo)
                .Include(v => v.Concessionaria)
                .AsNoTracking();

        public async Task<Venda?> ObterPorIdAsync(int id) =>
            await _context.Vendas
                .Include(v => v.Veiculo)
                .Include(v => v.Concessionaria)
                .FirstOrDefaultAsync(v => v.Id == id);

        public async Task<List<Veiculo>> ObterVeiculosPorConcessionariaAsync(int concessionariaId) =>
            await _context.Veiculos
                .Where(v => v.ConcessionariaId == concessionariaId)
                .AsNoTracking()
                .ToListAsync();

        public async Task AdicionarAsync(Venda venda) => await _context.Vendas.AddAsync(venda);
        public void Atualizar(Venda venda) => _context.Vendas.Update(venda);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync() =>
            _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitTransactionAsync() => await _transaction?.CommitAsync();
        public async Task RollbackTransactionAsync() => await _transaction?.RollbackAsync();

        public async Task<IEnumerable<Concessionaria>> ObterConcessionariasAsync()
        {
            return await _context.Concessionarias
                .AsNoTracking()
                .ToListAsync();
        }
    }

}
