using Microsoft.EntityFrameworkCore;
using Dima.Core.Models.Orders;

namespace Dima.API.Data.Mapping.OrderItens
{
    public class OrderMap : IEntityTypeConfiguration<Order>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Order");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderCode)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(8);

            builder.Property(x => x.ExternalReference)
                .HasColumnType("VARCHAR")
                .HasMaxLength(60);

            builder.Property(x => x.Gateway)
                .IsRequired()
                .HasColumnType("SMALLINT");

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamptz");

            builder.Property(x => x.Status)
                .IsRequired()
                .HasColumnType("smallint");

            builder.Property(x => x.UpdatedAt)
                .IsRequired()
                .HasColumnType("timestamptz");

            builder.Property(x => x.UserId)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(160);

            builder.HasOne(x => x.Voucher).WithMany();
            builder.HasOne(x => x.Product).WithMany();
        }
    }
}
