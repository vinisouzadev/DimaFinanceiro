using Dima.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.API.Data.Mapping
{
    public class CategoryMap : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> b)
        {
            b.ToTable("Category");

            b.HasKey(b => b.Id);

            b.Property(b => b.Title)
             .IsRequired()
             .HasColumnType("VARCHAR")
             .HasMaxLength(80);

            b.Property(b => b.Description)
             .HasColumnType("VARCHAR")
             .HasMaxLength(255);
            
            b.Property(b => b.UserId)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(160);
        }
    }
}
