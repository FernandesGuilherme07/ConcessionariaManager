using ConcessionariaManager.Core.Models;

namespace ConcessionariaManager.Core.Interfaces.Repositories
{
    public interface IFabricanteRepository
    {
        IQueryable<Fabricante> GetAll(string searchString);
        Task<Fabricante?> GetByIdAsync(int? id);
        Task<bool> ExistsByNameAsync(string nome, int? ignoreId = null);
        Task AddAsync(Fabricante fabricante);
        Task UpdateAsync(Fabricante fabricante);
        Task SoftDeleteAsync(int id);
    }
}
