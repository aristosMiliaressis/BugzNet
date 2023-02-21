using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;

namespace BugzNet.Core.Constants
{
    public static class BugzClaims
    {
        public const string MustChangePass = "mustChangePass";
        public const string SecondFactorRequied = "secondFactorRequied";
    }
}
