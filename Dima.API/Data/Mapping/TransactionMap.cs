using Dima.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.API.Data.Mapping
{
    public class TransactionMap : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> b)
        {
            b.ToTable("Transaction");

            b.HasKey(b => b.Id);

            b.Property(b => b.Title)
                .HasColumnType("VARCHAR")
                .HasMaxLength(80);

            b.Property(b => b.Type)
                .IsRequired()
                .HasColumnType("SMALLINT");

            b.Property(b => b.Amount)
                .IsRequired()
                .HasColumnType("MONEY");

            b.Property(b => b.CreatedAt)
                .IsRequired();

            b.Property(b => b.PaidOrReceivedAt);

            b.Property(b => b.UserId)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(160);
        }
    }
}
