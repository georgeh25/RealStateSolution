using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Configurations
{
    public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
    {
        public void Configure(EntityTypeBuilder<Owner> builder)
        {
            builder.ToTable("Owner");

            builder.HasKey(o => o.IdOwner);

            // Configure properties
            builder.Property(o => o.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(o => o.Address)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(o => o.Photo)
                   .HasMaxLength(500);

            builder.Property(o => o.Birthday)
                   .IsRequired();

            // Configure relationship
            builder.HasMany(o => o.Properties)
                   .WithOne(p => p.Owner)
                   .HasForeignKey(p => p.IdOwner)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
