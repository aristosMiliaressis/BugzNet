using BugzNet.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UAParser;

namespace BugzNet.Web.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var watch = Stopwatch.StartNew();

            var uaHeader = context.Request.Headers[HeaderNames.UserAgent];
            var uaParser = Parser.GetDefault();
            uaParser.TryParse(uaHeader, out string uaInfo);

            var ip = context.Connection.RemoteIpAddress.MapToIPv4().ToString();

            _logger.LogInformation("\"{Method} {PathBase}{Path}\" {Identity} {IP}", 
                context.Request.Method, context.Request.PathBase, context.Request.Path, 
                context.User.Identity.Name ?? "<unauthenticated>", /*uaInfo,*/ ip);

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                if (context.User.Identity.IsAuthenticated)
                    _logger.LogTrace("Claims:\r\n{Claims}", string.Join(Environment.NewLine, context.User.Claims.Select(c => c.Type+ "=" + c.Value)));

                _logger.LogTrace("Request:\r\n{Method} {PathBase}{Path}{QueryString}\r\n{Headers}", 
                    context.Request.Method, context.Request.PathBase,
                    context.Request.Path, context.Request.QueryString.ToUriComponent(),
                    string.Join(Environment.NewLine, context.Request.Headers.Select(h => h.Key +": " + h.Value)));
            }

            await _next(context);
            watch.Stop();

            _logger.LogInformation("Request {Status} in {ElapsedMilliseconds}ms {StatusCode} {ContentType}", 
                (context.RequestAborted.IsCancellationRequested ? "timed out" : "completed"),
                watch.ElapsedMilliseconds, context.Response.StatusCode, context.Response.ContentType);
        }
    }
}
