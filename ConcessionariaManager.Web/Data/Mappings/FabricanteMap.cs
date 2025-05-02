using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ConcessionariaManager.Core.Models;

namespace ConcessionariaManager.Web.Data.Mappings
{
    public sealed class FabricanteMap : IEntityTypeConfiguration<Fabricante>
    {
        public void Configure(EntityTypeBuilder<Fabricante> builder)
        {
            builder.ToTable("Fabricantes");
            
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(f => f.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(f => f.Nome)
                .IsUnique()
                .HasDatabaseName("IX_Fabricantes_Nome");

            builder.Property(f => f.PaisOrigem)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(f => f.AnoFundacao)
                .IsRequired();

            builder.Property(f => f.Website)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.CreatedAt)
                .IsRequired();

            builder.Property(f => f.UpdatedAt)
                .IsRequired();
            
            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
