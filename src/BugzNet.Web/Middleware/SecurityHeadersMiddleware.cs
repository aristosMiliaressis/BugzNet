using Microsoft.AspNetCore.Http;
using BugzNet.Infrastructure.Configuration;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BugzNet.Web.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, AppConfig config)
        {
            var randomBytes = new byte[16];
            _rng.GetBytes(randomBytes); 

            context.Items["CSP_NONCE"] = Convert.ToBase64String(randomBytes);

            context.Response.Headers.Add("Content-Security-Policy",
                "default-src 'self'; img-src 'self' data:;"
                + "script-src 'self' 'nonce-"+context.Items["CSP_NONCE"]+"';"
                + "style-src 'self' 'unsafe-inline';"
                + "frame-ancestors 'self';  frame-src 'self';"
                + "form-action 'self';  child-src 'none';"
                + "worker-src 'none'; object-src 'none'; base-uri 'self';"
                + (config.Sentry.CSPReporting ? "report-uri "+config.Sentry.CSPEndpoint+"; report-to default;" : ""));

            context.Response.Headers.Add("Report-To", "{ \"group\": \"default\", \"max_age\": 10886400, \"endpoints\": [{ \"url\": \"" + config.Sentry.CSPEndpoint + "\"}]}");

            context.Response.Headers.Add("X-Frame-Options", "DENY");

            context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");

            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

            context.Response.Headers.Add("Referrer-Policy", "no-referrer");

            await _next(context);
        }
    }
}
