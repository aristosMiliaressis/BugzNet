using BugzNet.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugzNet.Infrastructure.DataEF.Configurations
{
    public class OTPConfig : IEntityTypeConfiguration<OTP>
    {
        public void Configure(EntityTypeBuilder<OTP> builder)
        {
            builder.ConfigureBase();

            builder.ToTable(nameof(OTP));

            builder.HasKey(al => al.Id);

            // Each User can have many UserClaims
            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();
        }
    }
}
