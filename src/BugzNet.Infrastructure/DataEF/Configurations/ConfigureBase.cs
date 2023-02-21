
using BugzNet.Core.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugzNet.Infrastructure.DataEF
{
    public static class EntityBaseConfiguration
    {
        public static void ConfigureBase<TEntity>(this EntityTypeBuilder<TEntity> entity)
            where TEntity : BaseEntity<int>
        {

            entity.Property(i => i.CreatedOn)
              .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            entity.Property(e => e.CreatedOn)
                  .ValueGeneratedOnAdd()
                  .HasDefaultValueSql("GETDATE()");
        }
    }
}
