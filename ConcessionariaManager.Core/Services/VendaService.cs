using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Core.Interfaces.Services;
using ConcessionariaManager.Core.Models;

namespace ConcessionariaManager.Core.Services
{
    public class VendaService : IVendaService
    {
        private readonly IVendaRepository _repository;

        public VendaService(IVendaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<object>> ObterVeiculosDisponiveisJsonAsync(int concessionariaId)
        {
            var veiculos = await _repository.ObterVeiculosDisponiveisPorConcessionariaAsync(concessionariaId);
            return veiculos.Select(v => new { v.Id, v.Modelo }).Cast<object>().ToList();
        }

        public async Task<(bool Sucesso, string? Erro, Venda? Venda)> CriarVendaAsync(Venda venda)
        {
            venda.Cancelada = false;
            venda.ProtocoloVenda = Guid.NewGuid().ToString();
            venda.CPFCliente = venda.CPFCliente.Replace(".", "").Replace("-", "").Trim();
            var veiculo = await _repository.ObterVeiculoPorIdAsync(venda.VeiculoId);
            if (veiculo == null) return (false, "Veículo não encontrado.", null);

            if (veiculo.Vendido)
                return (false, "Veículo já vendido.", null);

            if (venda.PrecoVenda > veiculo.Preco)
                return (false, $"O preço da venda não pode ser maior que o preço do veículo (R$ {veiculo.Preco:N2}).", null);

            veiculo.Vendido = true;
            venda.Veiculo = veiculo;

            await _repository.RealizarVendaAsync(venda);
            return (true, null, venda);
            
        }

        public async Task<(bool Sucesso, string? Erro)> CancelarVendaAsync(int id, string motivo)
        {
            var venda = await _repository.ObterVendaPorIdAsync(id);
            if (venda == null || venda.Cancelada)
                return (false, "Venda inválida ou já cancelada.");

            if (DateTime.Now > venda.DataVenda.AddMonths(3))
                return (false, "A venda não pode ser cancelada após 3 meses da data da venda.");

            using var transaction = await _repository.BeginTransactionAsync();
            try
            {
                venda.Cancelada = true;
                venda.MotivoDoCancelameto = motivo;
                venda.Veiculo!.Vendido = false;

                await _repository.CancelarVendaAsync(venda);
                return (true, null);
            }
            catch
            {
                return (false, "Erro ao cancelar a venda.");
            }
        }
    }

}
