using BugzNet.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugzNet.Infrastructure.DataEF.Configurations.Identity
{
    public class BugzUserConfig : IEntityTypeConfiguration<BugzUser>
    {
        public void Configure(EntityTypeBuilder<BugzUser> builder)
        {
            builder.ToTable("BugzUsers");

            // Each User can have many UserClaims
            builder.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        }
    }

    public class BugzRoleConfig : IEntityTypeConfiguration<BugzRole>
    {
        public void Configure(EntityTypeBuilder<BugzRole> builder)
        {
            builder.ToTable("BugzRoles");
            // Each Role can have many entries in the UserRole join table
            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        }
    }

    public class BugzUserClaimConfig : IEntityTypeConfiguration<BugzUserClaim>
    {
        public void Configure(EntityTypeBuilder<BugzUserClaim> builder)
        {
            builder.ToTable(nameof(BugzUserClaim));

            // Each User can have many UserClaims
            builder.HasOne(e => e.User)
                .WithMany(e => e.Claims)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();
        }
    }

    public class BugzUserRoleConfig : IEntityTypeConfiguration<BugzUserRole>
    {
        public void Configure(EntityTypeBuilder<BugzUserRole> builder)
        {
            builder.ToTable(nameof(BugzUserRole));

            // Each User can have many UserClaims
            builder.HasOne(e => e.User)
                .WithMany(e => e.UserRoles)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();
        }
    }
}
