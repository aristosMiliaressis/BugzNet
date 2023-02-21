using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;

namespace BugzNet.Core.Entities.Identity
{
    public class BugzUserClaim : IdentityUserClaim<string>
    {
        public virtual BugzUser User { get; set; }
    }
}
