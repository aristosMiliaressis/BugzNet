using BugzNet.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugzNet.Infrastructure.DataEF.Configurations
{
    public class AuditLogConfig : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> entity)
        {
            entity.ConfigureBase();

            entity.ToTable(nameof(AuditLog));

            entity.HasKey(al => al.Id);

            entity.HasIndex(e => e.DaysSinceNineteenHundred);
        }
    }
}
