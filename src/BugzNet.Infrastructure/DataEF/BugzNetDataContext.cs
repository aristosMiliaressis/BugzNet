using System.Linq;
using BugzNet.Core.Entities;
using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Audit.EntityFramework;
using BugzNet.Core.Localization;
using BugzNet.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using BugzNet.Core.Utilities;
using BugzNet.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BugzNet.Core.Entities.Identity;

namespace BugzNet.Infrastructure.DataEF
{
    public class BugzNetDataContext : IdentityDbContext<BugzUser, BugzRole, string, IdentityUserClaim<string>, BugzUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        private static ILogger _logger => LoggingUtility.LoggerFactory.CreateLogger(nameof(BugzNetDataContext));
        private readonly IAuditDbContext _auditContext;
        private readonly DbContextHelper _helper = new DbContextHelper();
        private static DbContextOptionsBuilder _optionsBuilder;

        public BugzNetDataContext(DbContextOptions options)
            : base(options)
        {
            _auditContext = new DefaultAuditContext(this);
            _helper.SetConfig(_auditContext);
        }

        #region DbSets
        public virtual DbSet<OTP> OTP { get; set; }
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<ReportingOperation> ReportingOperations { get; set; }
        #endregion

        #region Overrides
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BugzNetDataContext).Assembly);

            //convert DateTime types to DateTimeOffset before storing in db, to maintain timezone info
            var dateTimeConverter = new ValueConverter<DateTime, DateTimeOffset>(v => LocalizationUtility.AddTimeZoneInfo(v), v => LocalizationUtility.DateTimeOffsetToLocalTime(v));
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()))
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(dateTimeConverter);
            }
        }

        public new int SaveChanges()
        {
            return _helper.SaveChanges(_auditContext,
            () =>
            {
                int result = base.SaveChanges();

                return result;
            });
        }

        public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _helper.SaveChangesAsync(_auditContext,
                   async () =>
                   {
                       int result = await base.SaveChangesAsync(cancellationToken);

                       return result;
                   });
        }

        public async Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try 
            {
                int res = await SaveChangesAsync(cancellationToken);
                return res > 0;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, critical: true);
                return false;
            }
        }
        #endregion

        #region Static Methods
        public static BugzNetDataContext Create()
        {
            return new BugzNetDataContext(GetOptionsBuilder().Options);
        }

        public static DbContextOptionsBuilder GetOptionsBuilder(DbContextOptionsBuilder builder = null)
        {
            if (_optionsBuilder != null)
                return _optionsBuilder;
            else if (builder == null)
                builder = new DbContextOptionsBuilder();

#if DEBUG
            var loggerFactory = LoggerFactory.Create(b => b.AddDebug());
            builder = builder.UseLoggerFactory(loggerFactory).EnableSensitiveDataLogging(true);
#endif

            IConfiguration config = ConfigurationFactory.BuildConfiguration();
            string connectionString = config.GetConnectionString(nameof(AppConfig.DefaultConnection));

            builder = builder.UseSqlServer(connectionString, sqlOptions => sqlOptions.CommandTimeout(180));

            _optionsBuilder = builder;
            return builder;
        }
        #endregion
    }
}