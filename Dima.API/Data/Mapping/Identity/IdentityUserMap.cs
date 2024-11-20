using Dima.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.API.Data.Mapping.Identity
{
    public class IdentityUserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> b)
        {
            b.ToTable("IdentityUser");
            b.HasKey(u => u.Id);

            b.HasIndex(u => u.NormalizedEmail).IsUnique();
            b.HasIndex(u => u.NormalizedUserName).IsUnique();

            b.Property(u => u.Email).HasColumnType("VARCHAR").HasMaxLength(180);
            b.Property(u => u.NormalizedEmail).HasColumnType("VARCHAR").HasMaxLength(180);
            b.Property(u => u.UserName).HasColumnType("VARCHAR").HasMaxLength(180).IsRequired();
            b.Property(u => u.NormalizedUserName).HasColumnType("VARCHAR").HasMaxLength(180);
            b.Property(u => u.PhoneNumber).HasColumnType("VARCHAR").HasMaxLength(20);
            b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

            b.HasMany<IdentityUserClaim<long>>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
            b.HasMany<IdentityUserLogin<long>>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
            b.HasMany<IdentityUserToken<long>>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();
            b.HasMany<IdentityUserRole<long>>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
        }
    }
}
