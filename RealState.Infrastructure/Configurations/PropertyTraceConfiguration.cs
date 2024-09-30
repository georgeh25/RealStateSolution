using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Configurations
{
    public class PropertyTraceConfiguration : IEntityTypeConfiguration<PropertyTrace>
    {
        public void Configure(EntityTypeBuilder<PropertyTrace> builder)
        {
            builder.ToTable("PropertyTrace");

            builder.HasKey(pt => pt.IdPropertyTrace);

            // Configure properties
            builder.Property(pt => pt.DateSale)
                   .IsRequired();

            builder.Property(pt => pt.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(pt => pt.Value)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(pt => pt.Tax)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();
        }
    }
}
