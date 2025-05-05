using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ConcessionariaManager.Core.Models.AppLog;

namespace ConcessionariaManager.Web.Data.Mappings
{
    public class LogMap : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable("Logs");

            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasMany(l => l.LogDetails)
                .WithOne(d => d.Log)
                .HasForeignKey(d => d.LogId);
        }
    }
}
