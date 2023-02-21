using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugzNet.Web.Pages.Shared;

namespace BugzNet.Web.Pages.Identity
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModelBase
    {
        public void OnGet()
        {
        }
    }
}
