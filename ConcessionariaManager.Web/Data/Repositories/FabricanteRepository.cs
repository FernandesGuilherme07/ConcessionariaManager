using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace ConcessionariaManager.Web.Data.Repositories
{
    public class FabricanteRepository : IFabricanteRepository
    {
        private readonly ApplicationDbContext _context;

        public FabricanteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Fabricante> GetAll(string searchString)
        {
            var query = _context.Fabricantes
                .AsNoTracking();
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(f => f.Nome.Contains(searchString));
            }
            return query.OrderByDescending(f => f.Nome);
        }

        public async Task<Fabricante?> GetByIdAsync(int? id)
        {
            return await _context.Fabricantes.FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
        }

        public async Task<bool> ExistsByNameAsync(string nome, int? ignoreId)
        {
            return await _context.Fabricantes
                .WhereIf(ignoreId, f => f.Id != ignoreId && f.Nome == nome)
                .AnyAsync();
        }

        public async Task AddAsync(Fabricante fabricante)
        {
            _context.Fabricantes.Add(fabricante);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Fabricante fabricante)
        {
            _context.Fabricantes.Update(fabricante);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var fabricante = await _context.Fabricantes.FindAsync(id);
            if (fabricante != null)
            {
                fabricante.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
