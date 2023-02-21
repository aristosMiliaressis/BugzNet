using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BugzNet.Core.Exceptions;
using BugzNet.Infrastructure.Exceptions;
using BugzNet.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BugzNet.Web.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (RequestValidationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            catch (OperationCanceledException)
            {
                _logger.LogCritical("Request Timed Out");

                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, critical: true);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
