using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Core.Models.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace ConcessionariaManager.Web.Data.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RelatorioVendaItemViewModel>> ObterDadosParaExportacao(DateTime dataInicio, DateTime dataFim)
        {
            var vendas = await _context.Vendas
                .Include(v => v.Veiculo)
                    .ThenInclude(v => v.Fabricante)
                .Include(v => v.Concessionaria)
                .Where(v => !v.IsDeleted && !v.Cancelada && v.DataVenda >= dataInicio && v.DataVenda <= dataFim)
                .ToListAsync();

           return vendas
                .GroupBy(v => new
                {
                    v.Veiculo!.TipoVeiculo,
                    Fabricante = v.Veiculo.Fabricante!.Nome,
                    Concessionaria = v.Concessionaria!.Nome
                })
                .Select(g => new RelatorioVendaItemViewModel
                {
                    TipoVeiculo = g.Key.TipoVeiculo.ToString(),
                    Fabricante = g.Key.Fabricante,
                    Concessionaria = g.Key.Concessionaria,
                    QuantidadeVendida = g.Count(),
                    TotalVendido = g.Sum(v => v.PrecoVenda)
                })
                .ToList();

        }

        public async Task<List<Venda>> ObterVendasAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.Vendas
                .AsNoTracking()
                .Include(v => v.Veiculo)
                    .ThenInclude(v => v.Fabricante)
                .Include(v => v.Concessionaria)
                .Where(v => !v.IsDeleted && !v.Cancelada && v.DataVenda >= dataInicio && v.DataVenda <= dataFim)
                .ToListAsync();
        }
    }

}
