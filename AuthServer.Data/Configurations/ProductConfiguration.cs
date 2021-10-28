using AuthServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m=>m.Name).IsRequired().HasMaxLength(50);
            builder.Property(m => m.Stock).IsRequired();
            builder.Property(m => m.Price).HasColumnType("decimal(18,2)");
            builder.Property(m => m.UserId).IsRequired();
        }
    }
}
