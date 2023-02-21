using BugzNet.Core.Entities.Identity;
using BugzNet.Core.SharedKernel;
using System;

namespace BugzNet.Core.Entities
{
    public class OTP : BaseEntity<int>
    {
        public static TimeSpan Expiration = new TimeSpan(0,2,0);
        
        public string Code { get; set; }
        public string UserId { get; set; }
        public virtual BugzUser User { get; set; }
    }
}
