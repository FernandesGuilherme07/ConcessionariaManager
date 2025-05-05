using ConcessionariaManager.Core.Models;
using System.Collections;

namespace ConcessionariaManager.Core.Interfaces.Services
{
    public interface IVendaService
    {
        Task<List<object>> ObterVeiculosDisponiveisJsonAsync(int concessionariaId);
        Task<(bool Sucesso, string? Erro, Venda? Venda)> CriarVendaAsync(Venda venda);
        Task<(bool Sucesso, string? Erro)> CancelarVendaAsync(int id, string motivo);
    }

}

