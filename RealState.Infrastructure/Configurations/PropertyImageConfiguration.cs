using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Configurations
{
    public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
    {
        public void Configure(EntityTypeBuilder<PropertyImage> builder)
        {
            builder.ToTable("PropertyImage");

            builder.HasKey(pi => pi.IdPropertyImage);

            // Configure properties
            builder.Property(pi => pi.File)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(pi => pi.Enabled)
                   .IsRequired();
        }
    }
}
