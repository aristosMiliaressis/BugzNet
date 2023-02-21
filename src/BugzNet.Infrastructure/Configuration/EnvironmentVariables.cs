using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Configuration
{
    public static class EnvironmentVariables
    {
        public static string ASPNETCORE_ENVIRONMENT => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
        public static string ASPNETCORE_URLS => Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? string.Empty;
        public static bool DOTNET_RUNNING_IN_CONTAINER => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")?.Equals("true") ?? false;
        public static bool USE_IN_MEMORY_DB => Environment.GetEnvironmentVariable("USE_IN_MEMORY_DB")?.Equals("true") ?? false;
        public static bool HARD_MODE => Environment.GetEnvironmentVariable("HARD_MODE")?.Equals("true") ?? false;
    }
}
