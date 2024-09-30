using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Configurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {           
            builder.ToTable("Property");

            builder.HasKey(p => p.IdProperty);

            // Configure properties
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Address)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(p => p.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(p => p.CodeInternal)
                   .HasMaxLength(50);

            builder.Property(p => p.Year)
                   .IsRequired();

            // Configure relationships
            builder.HasMany(p => p.PropertyImages)
                   .WithOne(pi => pi.Property)
                   .HasForeignKey(pi => pi.IdProperty)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PropertyTraces)
                   .WithOne(pt => pt.Property)
                   .HasForeignKey(pt => pt.IdProperty)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
