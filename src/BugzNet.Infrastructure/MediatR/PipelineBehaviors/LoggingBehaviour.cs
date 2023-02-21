using MediatR;
using Microsoft.Extensions.Logging;
using BugzNet.Infrastructure.MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.MediatR.PipelineBehaviors
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : class
    {
        private readonly ILogger<TRequest> _logger;

        public LoggingBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            using (_logger.BeginScope(request))
            {
                var requestName = request.GetType().Name.Split('.').Last();
                _logger.LogInformation("Calling handler {requestName}", requestName);

                var watch = Stopwatch.StartNew();
                var response = await next();
                watch.Stop();

                if (typeof(TResponse).IsAssignableFrom(typeof(CommandResponse))
                 || typeof(TResponse).IsAssignableFrom(typeof(CommandResponse<int>))
                 || typeof(TResponse).IsAssignableFrom(typeof(CommandResponse<string>))
                 || typeof(TResponse).IsAssignableFrom(typeof(CommandResponse<bool>)))
                {
                    _logger.LogInformation("{requestName} handler returned after {ElapsedMilliseconds}ms with result {response}", requestName, watch.ElapsedMilliseconds, JsonConvert.SerializeObject(response));
                    return response;
                }

                _logger.LogInformation("{requestName} handler returned after {ElapsedMilliseconds}ms", requestName, watch.ElapsedMilliseconds);
                return response;
            }
        }
    }
}
