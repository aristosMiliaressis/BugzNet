using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BugzNet.Core.Utilities;
using BugzNet.Infrastructure.Configuration.Models;
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BugzNet.Infrastructure.HostedServices
{
    public abstract class BaseHostedService : BackgroundService, IDisposable
    {
        protected string ServiceName => GetType().Name;
        protected virtual TimeSpan CallbackInterval { get; } = TimeSpan.FromMinutes(5);
        protected readonly ILogger _logger;
        
        private CancellationTokenSource _cts;
        private CancellationToken _processAbortedToken;
        private Timer _timer;

        protected BaseHostedService(List<WorkerConfig> config)
        {
            _logger = LoggingUtility.LoggerFactory.CreateLogger(ServiceName);
            if (config == null)
            {
                _logger.LogWarning("No configuration found for service {ServiceName}, using default config.", ServiceName);
                return;
            }

            var workerConfig = config.FirstOrDefault(w => w.Name == ServiceName);
            if (workerConfig.CallbackInterval <= 0)
            {
                _logger.LogWarning("Invalid configuration found for service {ServiceName}, using default config.", ServiceName);
                return;
            }

            CallbackInterval = TimeSpan.FromMinutes(workerConfig.CallbackInterval);
        }

        public new Task StartAsync(CancellationToken processAbortedToken)
        {
            _cts = new CancellationTokenSource();
            _processAbortedToken = processAbortedToken;
            _processAbortedToken.ThrowIfCancellationRequested();

            _timer = new Timer(_runCallbackWrapper, null, TimeSpan.Zero, CallbackInterval);

            return Task.CompletedTask;
        }

        public new Task StopAsync(CancellationToken processAbortedToken)
        {
            Dispose();

            return Task.CompletedTask;
        }

        public new void Dispose()
        {
            _timer?.Dispose();
            _cts?.Dispose();
        }

        private async void _runCallbackWrapper(object state)
        {
            var watch = Stopwatch.StartNew();

            _cts.CancelAfter(CallbackInterval);

            try
            {
                _logger.LogDebug("{ServiceName} started with interval {CallbackInterval}", ServiceName, CallbackInterval);

                await ExecuteAsync(_cts.Token);

                watch.Stop();
                _logger.LogDebug("{ServiceName} finished in {ElapsedMilliseconds}ms.", ServiceName, watch.ElapsedMilliseconds);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("{ServiceName} cancelled after {ElapsedMilliseconds}ms.", ServiceName, watch.ElapsedMilliseconds);
                if (_processAbortedToken.IsCancellationRequested)
                {
                    Dispose();
                }
            }
        }
    }
}
