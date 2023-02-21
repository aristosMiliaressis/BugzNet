using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;

namespace BugzNet.Core.Entities.Identity
{
    public class BugzRole : IdentityRole<string>
    {
        public string Description { get; set; }
        public virtual ICollection<BugzUserRole> UserRoles { get; set; }
    }
}
