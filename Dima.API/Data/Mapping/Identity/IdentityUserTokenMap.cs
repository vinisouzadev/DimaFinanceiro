using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.API.Data.Mapping.Identity
{
    public class IdentityUserTokenMap : IEntityTypeConfiguration<IdentityUserToken<long>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<long>> b)
        {
            b.ToTable("IdentityUserToken");
            b.HasKey(u => new {u.UserId, u.LoginProvider, u.Name});
            b.Property(u => u.LoginProvider).HasMaxLength(120);
            b.Property(u => u.Name).HasMaxLength(180);
        }
    }
}
