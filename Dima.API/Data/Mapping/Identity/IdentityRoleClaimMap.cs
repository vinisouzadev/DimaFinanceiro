using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.API.Data.Mapping.Identity
{
    public class IdentityRoleClaimMap : IEntityTypeConfiguration<IdentityRoleClaim<long>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<long>> b)
        {
            b.ToTable("IdentityRoleClaim");
            b.HasKey(u => u.Id);
            b.Property(u => u.ClaimType).HasMaxLength(255);
            b.Property(u => u.ClaimValue).HasMaxLength(255);
        }
    }
}
