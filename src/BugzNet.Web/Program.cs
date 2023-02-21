using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using BugzNet.Infrastructure.Configuration;
using BugzNet.Web.Extensions;
using BugzNet.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using BugzNet.Infrastructure.DataEF;
using NLog.StructuredLogging.Json;
using BugzNet.Core.Utilities;
using NLog.Extensions.Logging;

namespace BugzNet.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var config = ConfigurationFactory.BuildConfiguration();

            var logger = LogManager.Setup()
                       .LoadConfigurationFromSection(config)
                       .GetCurrentClassLogger();

            LoggingUtility.LoggerFactory = new NLogLoggerFactory();

            try
            {
                logger.ExtendedInfo($"BugzNet {AssemblyAttributes.Version}+{AssemblyAttributes.GitHash}", new { AssemblyAttributes.Version, AssemblyAttributes.GitHash });

                var host = CreateWebHostBuilder(args, config).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var dbInitializer = scope.ServiceProvider.GetService<DatabaseInitializer>();
                    await dbInitializer.InitializeAsync();
                }

                logger.ExtendedInfo($"Started listening on {EnvironmentVariables.ASPNETCORE_URLS}", new { EnvironmentVariables.ASPNETCORE_URLS });
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                logger.Extended(NLog.LogLevel.Fatal, $"Stopped program because of exception \r\n{ex.ToRecursiveStackTrace()}\r\n", new { Exception = ex });
            }
            finally
            {
                logger.ExtendedInfo("App Stopped");
                LogManager.Shutdown();
            }
        }

        internal static IHostBuilder CreateWebHostBuilder(string[] args, IConfiguration config)
            => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .ConfigureLogging((context, logging) =>
                    {
                        logging.ClearProviders();
                        // logging.AddNLog();
                        // logging.AddNLogWeb();
                    })
                    .UseNLog()
                    .UseStartup<Startup>()
                    .UseConfiguration(config)
                    .UseSentryWithProxy(config);
                });
    }   
}