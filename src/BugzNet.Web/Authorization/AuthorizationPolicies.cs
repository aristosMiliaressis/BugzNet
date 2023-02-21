using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BugzNet.Core.Constants;

namespace BugzNet.Web.Authorization
{
    public static class AuthorizationPolicies
    {
        public static AuthorizationPolicy SuperUserPolicy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireRole(BugzRoles.SuperUserRole)
            .Build();

        public static AuthorizationPolicy UIUserPolicy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                    .RequireRole(BugzRoles.SuperUserRole, BugzRoles.UserRole, BugzRoles.ReadOnlyRole)
                    .RequireAssertion(ctx =>
                    {
                        var filterContext = ctx.Resource as AuthorizationFilterContext;
                        if (filterContext != null && filterContext.HttpContext != null && filterContext.HttpContext.User.IsInRole(BugzRoles.ReadOnlyRole))
                        {
                            var httpMethod = filterContext.HttpContext.Request.Method;
                            if (httpMethod.ToUpper() == "POST")
                            {
                                var readOnlyUserPostWhiteList = new List<PathString> {
                                    new PathString("/Bugs/Error"),
                                    new PathString("/Identity/Login"),
                                    new PathString("/Identity/ForgotPassword"),
                                    new PathString("/Identity/ResetPassword"),
                                    new PathString("/MyAccount/ChangePassword")
                                };
                                return readOnlyUserPostWhiteList.Contains(filterContext.HttpContext.Request.Path);
                            }
                        }

                        return true;
                    })
                    .Build();

        public static AuthorizationPolicy JwtApiPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireRole(BugzRoles.ApiUserRole)
                    .Build();

        public static Func<string, AuthorizationPolicy> ApiKeyPolicy = (apiKey) => new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .AddRequirements(new ApiKeyRequirement(apiKey))
                    .Build();
    }
}
