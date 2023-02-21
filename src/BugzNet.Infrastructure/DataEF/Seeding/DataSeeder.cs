using BugzNet.Core.Entities;
using BugzNet.Core.Extensions;
using BugzNet.Core.Localization;
using BugzNet.Infrastructure.DataJson;
using BugzNet.Infrastructure.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using BugzNet.Core.Entities.Identity;
using BugzNet.Core.Constants;
using BugzNet.Infrastructure.Configuration;

namespace BugzNet.Infrastructure.DataEF.Seeding
{
    public class DataSeeder
    {
        private readonly BugzNetDataContext _context;
        private readonly SeedConfiguration _seedData;
        private readonly UserManager<BugzUser> _userMngr;
        private readonly RoleManager<BugzRole> _roleMngr;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_context"></param>
        /// <param name="seed"></param>
        public DataSeeder(BugzNetDataContext context, SeedConfiguration seed, 
            UserManager<BugzUser> userMngr, RoleManager<BugzRole> roleMngr)
        {
            _context = context;
            _seedData = seed;
            _userMngr = userMngr;
            _roleMngr = roleMngr;
        }

        public async Task SeedAllAsync()
        {
            // if there are already users in db assume that seed has already run
            var userCount = _context.Users.Count();
            if (userCount != 0)
                return;

            await SeedRolesAsync();
            await SeedUsersAsync();

            _context.SaveChanges();
        }
        private async Task SeedUsersAsync()
        {
            if (_userMngr.FindByNameAsync("BugzAdmin").Result == null)
            {
                BugzUser user = new BugzUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "admin@bugznet.com",
                    PhoneNumber = "6987654321",
                    Email = "admin@bugznet.com",
                    LockoutEnabled = false,
                    MustChangePassword = false,
                };

                IdentityResult result = await _userMngr.CreateAsync(user, "!bugzn3t@admin");

                if (result.Succeeded)
                {
                    await _userMngr.AddToRoleAsync(user, BugzRoles.SuperUserRole);
                }
            }

            if (_userMngr.FindByNameAsync("BugzUser1").Result == null)
            {
                BugzUser user = new BugzUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "user@bugznet.com",
                    Email = "user@bugznet.com",
                    PhoneNumber = "6913371337",
                    LockoutEnabled = false,
                    MustChangePassword = false,
                    TwoFactorEnabled = EnvironmentVariables.HARD_MODE
                };

                IdentityResult result = await _userMngr.CreateAsync(user, "!bugzn3t@user");

                if (result.Succeeded)
                {
                    await _userMngr.AddToRoleAsync(user, BugzRoles.UserRole);
                }
            }

            if (_userMngr.FindByNameAsync("ApiUser").Result == null)
            {
                BugzUser user = new BugzUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "apiuser",
                    Email = "api@bugznet.com",
                    LockoutEnabled = false,
                    MustChangePassword = false
                };

                IdentityResult result = await _userMngr.CreateAsync(user, "!bugzn3t@api");

                if (result.Succeeded)
                {
                    await _userMngr.AddToRoleAsync(user, BugzRoles.ApiUserRole);
                }
            }
        }

        private async Task SeedRolesAsync()
        {
            Type objType = typeof(BugzRoles);

            var roleProperties = objType.GetFields(BindingFlags.Public | BindingFlags.Static).ToList();

            foreach (var fInfo in roleProperties)
            {
                string roleName = (string)fInfo.GetRawConstantValue();
                if (!_roleMngr.RoleExistsAsync(roleName).Result)
                {
                    BugzRole role = new BugzRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = roleName,
                        Description = fInfo.Name
                    };
                    _ = await _roleMngr.CreateAsync(role);
                }
            }
        }
    }
}
