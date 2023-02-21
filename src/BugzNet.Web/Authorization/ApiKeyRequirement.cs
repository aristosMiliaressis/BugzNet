using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace BugzNet.Web.Authorization
{
    public class ApiKeyRequirement : IAuthorizationHandler, IAuthorizationRequirement
    {
        private readonly string _apiKey;

        public ApiKeyRequirement(string apiKey)
        {
            _apiKey = apiKey;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.Resource is HttpContext httpContext)
            {
                var authHeader = httpContext.Request.Headers["Authorization"].ToString();
                if (authHeader != null 
                 && authHeader.Split(' ')[0].Equals("bearer", StringComparison.InvariantCultureIgnoreCase)
                 && authHeader.Split(' ')[1].Equals(_apiKey))
                {
                    context.Succeed(this);
                }
            }

            return Task.CompletedTask;
        }
    }
}
