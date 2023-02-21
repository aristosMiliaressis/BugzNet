using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Configuration
{
    public class ConnectionString
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public string GetCatalogName()
        {
            var parsedConnStr = Value.Split(';').Where(p => !string.IsNullOrEmpty(p)).ToDictionary(p => p.Split('=')[0].ToLower(), p => p.Split('=')[1]);

            return parsedConnStr.ContainsKey("database") ? parsedConnStr["database"] : parsedConnStr["initial catalog"];
        }
    }
}
