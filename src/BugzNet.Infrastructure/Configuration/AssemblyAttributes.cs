using Microsoft.Extensions.Configuration;
using BugzNet.Infrastructure.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Configuration
{
    public static class AssemblyAttributes
    {
        private static readonly Assembly _startupAssembly = Assembly.GetEntryAssembly();

        public static readonly Assembly[] CustomAssemblies = _startupAssembly
                                                                        .GetReferencedAssemblies()
                                                                        .Where(an => an.FullName.StartsWith("BugzNet"))
                                                                        .Select(an => Assembly.Load(an)).ToArray();

        public static readonly string GitTag = _startupAssembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(attr => attr.Key == "GitTag")?.Value;

        public static readonly string GitHash = _startupAssembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(attr => attr.Key == "GitHash")?.Value;

        public static readonly string Version = _startupAssembly.GetName().Version.ToString();
    }
}
