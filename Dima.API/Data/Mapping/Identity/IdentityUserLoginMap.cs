using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.API.Data.Mapping.Identity
{
    public class IdentityUserLogin : IEntityTypeConfiguration<IdentityUserLogin<long>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<long>> b)
        {
            b.ToTable("IdentityUserLogin");
            b.HasKey(u => new { u.LoginProvider, u.ProviderKey });
            b.Property(u => u.LoginProvider).HasMaxLength(128);
            b.Property(u => u.ProviderKey).HasMaxLength(128);
            b.Property(u => u.ProviderDisplayName).HasMaxLength(255);
        }
    }
}
