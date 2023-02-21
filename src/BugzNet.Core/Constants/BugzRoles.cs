using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;

namespace BugzNet.Core.Constants
{
    public static class BugzRoles
    {
        public const string SuperUserRole = "SuperUser";
        public const string UserRole = "User";
        public const string ReadOnlyRole = "ReadOnlyUser";
        public const string ApiUserRole = "ApiUser";
    }
}
