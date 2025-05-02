using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Core.Enums;

namespace ConcessionariaManager.Web.Infrastructure.Persistence.Mappings
{
    public sealed class VeiculoMap : IEntityTypeConfiguration<Veiculo>
    {
        public void Configure(EntityTypeBuilder<Veiculo> builder)
        {
            builder.ToTable("Veiculos");

            builder.HasKey(v => v.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(v => v.Modelo)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(v => v.Modelo)
                .HasDatabaseName("IX_Veiculos_Modelo");

            builder.Property(v => v.AnoFabricacao)
                .IsRequired();

            builder.HasIndex(v => v.AnoFabricacao)
                .HasDatabaseName("IX_Veiculos_AnoFabricacao");

            builder.Property(v => v.Placa)
                .IsRequired();

            builder.Property(v => v.Preco)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(v => v.TipoVeiculo)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<TipoVeiculo>(v))
                .IsRequired();

            builder.HasIndex(v => v.TipoVeiculo)
                .HasDatabaseName("IX_Veiculos_TipoVeiculo");

            builder.Property(v => v.FabricanteId)
                .IsRequired();

            builder.HasOne(v => v.Fabricante)
                .WithMany(f => f.Veiculos)
                .HasForeignKey(v => v.FabricanteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Concessionaria)
                .WithMany(f => f.Veiculos)
                .HasForeignKey(v => v.ConcessionariaId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(v => v.Descricao)
                .HasMaxLength(500);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
