using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BugzNet.Core.Utilities;
using BugzNet.Infrastructure.Exceptions;
using System;
using System.IO;

namespace BugzNet.Infrastructure.Configuration
{
    public static class ConfigurationFactory
    {
        private readonly static ILogger _logger = LoggingUtility.LoggerFactory.CreateLogger(nameof(ConfigurationFactory));

        public static IConfiguration BuildConfiguration()
        {
            var configBasePath = AppContext.BaseDirectory;
            var configFileName = $"appsettings.{EnvironmentVariables.ASPNETCORE_ENVIRONMENT}.json";
            var configFullPath = Path.Combine(configBasePath, configFileName);
            try
            {
                if (!File.Exists(configFullPath))
                    throw new FileNotFoundException(configFullPath);

                var config = new ConfigurationBuilder()
                        .SetBasePath(configBasePath)
                        .AddJsonFile(configFileName, optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();

                return config;
            }
            catch (Exception ex)
            {
                throw new StartupException($"Exception caught while reading config file: {configFullPath}", ex);
            }            
        }

        public static AppConfig CreateAppConfigModel(IConfiguration configRoot)
        { 
            var appConfig = configRoot.Get<AppConfig>();
            if (appConfig == null)
            {
                throw new StartupException("Failed to deserialize the config file");
            }

            if (string.IsNullOrEmpty(appConfig.BasePath))
                appConfig.BasePath = $"/";

            if (string.IsNullOrEmpty(appConfig.TokenSigningKey))
            {
                throw new StartupException("configuration field TokenSigningKey is missing.");
            }
            else if (appConfig.TokenSigningKey.StartsWith("[ENC]"))
            {
                appConfig.TokenSigningKey = CryptoUtility.Decypt(appConfig.TokenSigningKey.Substring(5));
            }

            if (string.IsNullOrEmpty(appConfig.WebRequestStaticToken))
            {
                _logger.LogWarning("configuration field WebRequestStaticToken is required for api endpoints.");
            }
            else if (appConfig.WebRequestStaticToken.StartsWith("[ENC]"))
            {
                appConfig.WebRequestStaticToken = CryptoUtility.Decypt(appConfig.WebRequestStaticToken.Substring(5));
            }

            if (!appConfig.ConnectionStrings.ContainsKey(nameof(appConfig.DefaultConnection)))
            {
                throw new StartupException("configuration field ConnectionStrings.DefaultConnection is missing.");
            }

            var decryptedConnectionString = appConfig.ConnectionStrings[nameof(appConfig.DefaultConnection)].StartsWith("[ENC]") 
                ? CryptoUtility.Decypt(appConfig.ConnectionStrings[nameof(appConfig.DefaultConnection)].Substring(5)) 
                : appConfig.ConnectionStrings[nameof(appConfig.DefaultConnection)];

            appConfig.ConnectionStrings[nameof(appConfig.DefaultConnection)] = decryptedConnectionString;
            appConfig.DefaultConnection = new ConnectionString
            {
                Name = nameof(appConfig.DefaultConnection),
                Value = decryptedConnectionString
            };

            if (appConfig.EmailSender.Password == null)
            {
                _logger.LogWarning("configuration field EmailSender.Password is missing.");
            }
            else if (appConfig.EmailSender.Password.StartsWith("[ENC]"))
            {
                appConfig.EmailSender.Password = CryptoUtility.Decypt(appConfig.EmailSender.Password.Substring(5));
            }

            return appConfig;
        }
    }
}
