using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Core.Interfaces.Services;
using ConcessionariaManager.Core.Interfaces;
using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Core.Utils;
using System.Collections;

namespace ConcessionariaManager.Core.Services
{
    public class VendaService : IVendaService
    {
        private readonly IVendaRepository _repository;

        public VendaService(IVendaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IPagedList<Venda>> ObterVendasPaginadasAsync(string? searchString, int pageNumber, int pageSize)
        {
            var query = _repository.ObterVendas();

            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(v => v.NomeCliente.Contains(searchString));

            return await PagedList<Venda>.CreateAsync(query.OrderBy(v => v.DataVenda), pageNumber, pageSize);
        }

        public async Task<List<Veiculo>> ObterVeiculosPorConcessionariaAsync(int concessionariaId) =>
            await _repository.ObterVeiculosPorConcessionariaAsync(concessionariaId);

        public async Task<Venda?> ObterDetalhesVendaAsync(int id) =>
            await _repository.ObterPorIdAsync(id);

        public async Task<(bool sucesso, string? erro)> CriarVendaAsync(Venda venda)
        {
            venda.Cancelada = false;
            venda.ProtocoloVenda = Guid.NewGuid().ToString();

            var veiculo = _repository.ObterVendas()
                .Select(v => v.Veiculo)
                .FirstOrDefault(v => v.Id == venda.VeiculoId);

            if (veiculo == null)
                return (false, "Veículo não encontrado.");

            if (venda.PrecoVenda > veiculo.Preco)
                return (false, $"Preço da venda não pode ser maior que o preço do veículo (R$ {veiculo.Preco:N2})");

            await _repository.BeginTransactionAsync();
            try
            {
                veiculo.Vendido = true;
                await _repository.AdicionarAsync(venda);
                await _repository.SaveChangesAsync();
                await _repository.CommitTransactionAsync();
                return (true, null);
            }
            catch
            {
                await _repository.RollbackTransactionAsync();
                return (false, "Erro ao realizar a venda.");
            }
        }

        public async Task<(bool sucesso, string? erro)> CancelarVendaAsync(int id, string motivo)
        {
            var venda = await _repository.ObterPorIdAsync(id);
            if (venda == null || venda.Cancelada) return (false, null);

            if (DateTime.Now > venda.DataVenda.AddMonths(3))
                return (false, "Venda não pode ser cancelada após 3 meses.");

            venda.Cancelada = true;
            venda.MotivoDoCancelameto = motivo;
            if (venda.Veiculo != null)
                venda.Veiculo.Vendido = false;

            await _repository.BeginTransactionAsync();
            try
            {
                _repository.Atualizar(venda);
                await _repository.SaveChangesAsync();
                await _repository.CommitTransactionAsync();
                return (true, null);
            }
            catch
            {
                await _repository.RollbackTransactionAsync();
                return (false, "Erro ao cancelar a venda.");
            }
        }

        public async Task<IEnumerable<Concessionaria>> ObterConcessionariasAsync()
        {
            return await _repository.ObterConcessionariasAsync();
        }
    }

}
