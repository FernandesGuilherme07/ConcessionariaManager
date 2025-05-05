using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Core.Models.AccessDeniedLog;
using ConcessionariaManager.Core.Models.AppLog;
using ConcessionariaManager.Web.Data.Mappings;
using ConcessionariaManager.Web.Infrastructure.Persistence.Mappings;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConcessionariaManager.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, LogSaveChangesInterceptor logInterceptor)
        : base(options)
    {
        _logInterceptor = logInterceptor;
    }

    public DbSet<Fabricante> Fabricantes { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }
    public DbSet<Venda> Vendas { get; set; }
    public DbSet<Concessionaria> Concessionarias { get; set; }
    public DbSet<AccessDeniedLog> AccessDeniedLogs { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<LogDetail> LogDetails { get; set; }

    private readonly LogSaveChangesInterceptor _logInterceptor;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new FabricanteMap());
        modelBuilder.ApplyConfiguration(new VeiculoMap());
        modelBuilder.ApplyConfiguration(new ConcessionariaMap());
        modelBuilder.ApplyConfiguration(new VendaMap());
        modelBuilder.ApplyConfiguration(new LogMap());

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_logInterceptor);
    }

}
