using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ConcessionariaManager.Core.Models;

namespace ConcessionariaManager.Web.Data.Mappings
{
    public class ConcessionariaMap : IEntityTypeConfiguration<Concessionaria>
    {
        public void Configure(EntityTypeBuilder<Concessionaria> builder)
        {
            builder.ToTable("Concessionarias");

            builder.HasKey(c => c.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => c.Nome)
                .IsUnique()
                .HasDatabaseName("IX_Concessionarias_Nome");

            builder.Property(c => c.Telefone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(f => f.Email)
                .IsUnique();

            builder.Property(c => c.CapacidadeMaximaDeVeiculos)
                .IsRequired();

            builder.OwnsOne(c => c.Endereco, endereco =>
            {
                endereco.Property(e => e.CEP)
                    .HasColumnName("Cep")
                    .IsRequired()
                    .HasMaxLength(9); 

                endereco.Property(e => e.Rua)
                    .HasColumnName("Rua")
                    .IsRequired()
                    .HasMaxLength(100);

                endereco.Property(e => e.Numero)
                    .HasColumnName("Numero")
                    .HasMaxLength(20);

                endereco.Property(e => e.Complemento)
                    .HasColumnName("Complemento")
                    .HasMaxLength(50);

                endereco.Property(e => e.Bairro)
                    .HasColumnName("Bairro")
                    .IsRequired()
                    .HasMaxLength(100);

                endereco.Property(e => e.Cidade)
                    .HasColumnName("Cidade")
                    .IsRequired()
                    .HasMaxLength(100);

                endereco.Property(e => e.EnderecoCompleto)
                    .HasColumnName("EnderecoCompleto")
                    .IsRequired()
                    .HasMaxLength(255);

                endereco.HasIndex(v => v.EnderecoCompleto)
                    .HasDatabaseName("IX_Endereco_EnderecoCompleto");

                endereco.Property(e => e.Estado)
                    .HasColumnName("Estado")
                    .IsRequired()
                    .HasMaxLength(2);

                endereco.WithOwner();
            });

            builder.Property(c => c.CreatedAt);

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
