using Dima.Core.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.API.Data.Mapping.OrderItens
{
    public class VourcherMap : IEntityTypeConfiguration<Voucher>
    {
        public void Configure(EntityTypeBuilder<Voucher> builder)
        {
            builder.ToTable("Voucher");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.VourcherCode)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(8);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(80);

            builder.Property(x => x.Description)
                .HasColumnType("VARCHAR")
                .HasMaxLength(255);

            builder.Property(x => x.Amount)
                .HasColumnType("Money")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnType("boolean")
                .IsRequired();

            builder.HasIndex(x => x.VourcherCode)
                .IsUnique();
        }
    }
}
