using BugzNet.Application.Requests.Identity.Commands;
using BugzNet.Core.Constants;
using BugzNet.Core.Utilities;
using BugzNet.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BugzNet.Web.Middleware
{
    public class MustChangePasswordMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppConfig _config;

        public MustChangePasswordMiddleware(RequestDelegate next, AppConfig config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated &&
                context.Request.Path != new PathString("/MyAccount/ChangePassword") &&
                context.Request.Path != new PathString("/MyAccount/Verify") &&
                context.Request.Path != new PathString("/Identity/Login"))
            {
                var cookie = context.Request.Cookies[AuthState.CookieName];
                var value = cookie.Split(".")[0];
                var signature = cookie.Split(".")[1];

                if (CryptoUtility.Sign(value, _config.HMACSecret) != signature)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                var state = JsonConvert.DeserializeObject<AuthState>(Encoding.UTF8.GetString(Convert.FromBase64String(value)), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                if (state.PasswordChangeRequired)
                {
                    var returnUrl = context.Request.Path.Value == "/" ? "" : "?returnUrl=" + HttpUtility.UrlEncode(context.Request.Path.Value);
                    context.Response.Redirect(context.Request.PathBase + "/MyAccount/ChangePassword" + returnUrl);
                }
            }
            await _next(context);
        }
    }
}
