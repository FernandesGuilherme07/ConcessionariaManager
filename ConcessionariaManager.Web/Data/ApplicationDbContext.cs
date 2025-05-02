using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Core.Models.AccessDeniedLog;
using ConcessionariaManager.Web.Data.Mappings;
using ConcessionariaManager.Web.Infrastructure.Persistence.Mappings;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConcessionariaManager.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Fabricante> Fabricantes { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }

    public DbSet<Venda> Vendas { get; set; }
    public DbSet<Concessionaria> Concessionarias { get; set; }
    public DbSet<AccessDeniedLog> AccessDeniedLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new FabricanteMap());
        modelBuilder.ApplyConfiguration(new VeiculoMap());
        modelBuilder.ApplyConfiguration(new ConcessionariaMap());
        modelBuilder.ApplyConfiguration(new VendaMap());

    }
}
