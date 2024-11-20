using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.API.Data.Mapping.Identity
{
    public class IdentityUserRoleMap : IEntityTypeConfiguration<IdentityUserRole<long>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<long>> b)
        {
            b.ToTable("IdentityUserRole");
            b.HasKey(ur => new {ur.UserId, ur.RoleId});
        }
    }
}
