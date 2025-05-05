using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using ConcessionariaManager.Core.Utils;
using NuGet.Protocol.Core.Types;

namespace ConcessionariaManager.Web.Data.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly ApplicationDbContext _context;

        public VendaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Venda> ObterVendas(string modelo, string cliente) => _context.Vendas
                .Include(v => v.Veiculo)
                .Include(v => v.Concessionaria)
                .WhereIf(cliente, v => v.NomeCliente.Contains(cliente))
                .WhereIf(modelo, v => v.Veiculo.Modelo.Contains(modelo));

        public IQueryable<Venda> ObterVendas() => _context.Vendas;

        public IQueryable<Veiculo> ObterVeiculosList() => _context.Veiculos;

        public IQueryable<Concessionaria> ObterConcessionariasList() => _context.Concessionarias;
        public async Task<Venda?> ObterVendaPorIdAsync(int id)
            => await _context.Vendas.Include(v => v.Veiculo).Include(v => v.Concessionaria).FirstOrDefaultAsync(v => v.Id == id);

        public async Task<List<Veiculo>> ObterVeiculosDisponiveisPorConcessionariaAsync(int concessionariaId)
            => await _context.Veiculos
                    .Where(v => v.ConcessionariaId == concessionariaId && !v.Vendido)
                    .ToListAsync();

        public async Task<Veiculo?> ObterVeiculoPorIdAsync(int id)
            => await _context.Veiculos.FirstOrDefaultAsync(v => v.Id == id);

        public async Task AdicionarVendaAsync(Venda venda)
            => await _context.Vendas.AddAsync(venda);

        public Task AtualizarVendaAsync(Venda venda)
        {
            _context.Vendas.Update(venda);
            return Task.CompletedTask;
        }

        public Task AtualizarVeiculoAsync(Veiculo veiculo)
        {
            _context.Veiculos.Update(veiculo);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<IDisposable> BeginTransactionAsync()
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            return transaction;
        }

        public async Task CancelarVendaAsync(Venda venda)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Vendas.Update(venda);
                _context.Veiculos.Update(venda.Veiculo!);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        public async Task RealizarVendaAsync(Venda venda)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Vendas.Add(venda);
                _context.Veiculos.Update(venda.Veiculo!);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch
            {
                transaction.Rollback();
            }
        }
    }

}
