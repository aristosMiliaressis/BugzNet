using BugzNet.Infrastructure.DataEF.Seeding;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BugzNet.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BugzNet.Core.Entities.Identity;

namespace BugzNet.Infrastructure.DataEF
{
    public class DatabaseInitializer
    {
        private readonly BugzNetDataContext _context;
        private readonly UserManager<BugzUser> _userMngr;
        private readonly RoleManager<BugzRole> _roleMngr;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatabaseInitializer(BugzNetDataContext context, UserManager<BugzUser> userMngr, 
            IHttpContextAccessor httpContextAccessor, RoleManager<BugzRole> roleMngr)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userMngr = userMngr;
            _roleMngr = roleMngr;
        }

        public async Task InitializeAsync()
        {
            try
            {
                AuditLogInitializer.Initialize(_httpContextAccessor);

                var jsonSeed = File.ReadAllText(Path.Combine($"{AppContext.BaseDirectory}/App_Data", "seed_data.json"));
                var seedConfig = JsonConvert.DeserializeObject<SeedConfiguration>(jsonSeed);

                if (seedConfig.DropAndRecreate)
                {
                    await _context.Database.EnsureDeletedAsync();
                    await _context.Database.EnsureCreatedAsync();
                    if (!_context.Database.IsInMemory())
                    {
                        _context.Database.SetCommandTimeout(10000);
                        await _context.Database.MigrateAsync();
                        _context.Database.SetCommandTimeout(180);
                    }
                }
                else if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.SetCommandTimeout(10000);
                    await _context.Database.MigrateAsync();
                    _context.Database.SetCommandTimeout(180);
                }

                if (!_context.Users.Any())
                {
                    var dbSeeder = new DataSeeder(_context, seedConfig, _userMngr, _roleMngr);
                    await dbSeeder.SeedAllAsync();
                }
            }
            catch (Exception ex)
            {
                throw new StartupException("Caught exception while initializing database", ex);
            }
        }
    }
}
