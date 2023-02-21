using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;

namespace BugzNet.Core.Entities.Identity
{
    public class BugzUserRole : IdentityUserRole<string>
    {
        public virtual BugzRole Role { get; set; }
        public virtual BugzUser User { get; set; }
    }
}
