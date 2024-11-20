using Dima.Core.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.API.Data.Mapping.OrderItens
{
    public class ProductMap : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(80);

            builder.Property(x => x.Title)
                .HasColumnType("VARCHAR")
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnType("VARCHAR")
                .HasMaxLength(255);

            builder.Property(x => x.Price)
                .HasColumnType("Money")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasColumnType("boolean");

            builder.HasIndex(x => x.Slug)
                .IsUnique();
        }
    }
}
