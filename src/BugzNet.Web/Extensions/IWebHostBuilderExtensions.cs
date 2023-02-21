using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BugzNet.Core.Utilities;
using BugzNet.Infrastructure.Configuration;
using BugzNet.Infrastructure.Extensions;
using Sentry;
using Sentry.AspNetCore;
using System;

namespace BugzNet.Web.Extensions
{
    public static class IWebHostBuilderExtensions
    {
        private readonly static ILogger _logger = LoggingUtility.LoggerFactory.CreateLogger(nameof(IWebHostBuilderExtensions));

        public static IWebHostBuilder UseSentryWithProxy(this IWebHostBuilder builder, IConfiguration config)
        {
            try
            {
                var sentryConfig = config.GetSection("Sentry").Get<SentryConfig>() ?? new SentryConfig();
                var proxySettings = config.GetSection("ProxySettings").Get<ProxySettings>() ?? new ProxySettings();

                if (sentryConfig.Enabled)
                {
                    builder.UseSentry((SentryAspNetCoreOptions o) =>
                    {
                        o.Dsn = sentryConfig.Dsn;
                        o.Environment = EnvironmentVariables.ASPNETCORE_ENVIRONMENT;
                        o.Release = $"{AssemblyAttributes.Version}+{AssemblyAttributes.GitHash}";
                        o.MinimumEventLevel = LogLevel.Critical;
                        o.SendDefaultPii = true;

                        if (proxySettings.Enabled)
                        {
                            o.HttpProxy = WebProxyFactory.Create(proxySettings);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Exception caught while setting up sentry, continuing without it.");
                _logger.LogException(ex);
            }           

            return builder;
        }
    }
}
