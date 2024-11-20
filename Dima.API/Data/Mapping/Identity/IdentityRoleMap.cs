using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.API.Data.Mapping.Identity
{
    public class IdentityRoleMap : IEntityTypeConfiguration<IdentityRole<long>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<long>> b)
        {
            b.ToTable("IdentityRole");
            b.HasKey(u => u.Id);
            b.HasIndex(u => u.NormalizedName).IsUnique();
            b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();
            b.Property(u => u.Name).HasMaxLength(256);
            b.Property(u => u.NormalizedName).HasMaxLength(256);
        }
    }
}
