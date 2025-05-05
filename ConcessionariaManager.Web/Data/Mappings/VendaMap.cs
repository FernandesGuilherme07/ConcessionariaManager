using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ConcessionariaManager.Core.Models;

namespace ConcessionariaManager.Web.Data.Mappings
{
    public class VendaMap : IEntityTypeConfiguration<Venda>
    {
        public void Configure(EntityTypeBuilder<Venda> builder)
        {
            builder.ToTable("Vendas");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id)
                .ValueGeneratedOnAdd();

            builder.Property(v => v.ConcessionariaId)
                .IsRequired();

            builder.HasOne(v => v.Concessionaria)
                .WithMany() 
                .HasForeignKey(v => v.ConcessionariaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(v => v.VeiculoId)
                .IsRequired();

            builder.HasOne(v => v.Veiculo)
                .WithMany() 
                .HasForeignKey(v => v.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(v => v.NomeCliente)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.CPFCliente)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(v => v.Telefone)
                .HasColumnName("TelefoneCliente")
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(v => v.DataVenda)
                .IsRequired();

            builder.Property(v => v.PrecoVenda)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(v => v.Cancelada)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(v => v.ProtocoloVenda)
                .HasMaxLength(50);

            builder.Property(v => v.CreatedAt);

            builder.Property(v => v.UpdatedAt);

            builder.Property(v => v.Cancelada)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
