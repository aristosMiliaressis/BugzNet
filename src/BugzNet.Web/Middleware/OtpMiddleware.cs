using BugzNet.Core.Constants;
using BugzNet.Core.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace BugzNet.Web.Middleware
{
    public class OtpMiddleware
    {
        private readonly RequestDelegate _next;

        public OtpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated &&
                context.Request.Path != new PathString("/MyAccount/Verify") &&
                context.Request.Path != new PathString("/Identity/Login") &&
                ((ClaimsIdentity)context.User.Identity).HasClaim(c => c.Type == BugzClaims.SecondFactorRequied))
            {
                var returnUrl = context.Request.Path.Value == "/" ? "" : "?returnUrl=" + HttpUtility.UrlEncode(context.Request.Path.Value);
                context.Response.Redirect(context.Request.PathBase+"/MyAccount/Verify" + returnUrl);
            }

            await _next(context);
        }
    }
}
