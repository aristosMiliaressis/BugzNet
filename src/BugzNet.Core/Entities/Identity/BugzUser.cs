using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;

namespace BugzNet.Core.Entities.Identity
{
    public class BugzUser : IdentityUser<string>
    {
        public bool MustChangePassword { get; set; }
        public virtual ICollection<BugzUserClaim> Claims { get; set; }
        public virtual ICollection<BugzUserRole> UserRoles { get; set; }
    }
}
