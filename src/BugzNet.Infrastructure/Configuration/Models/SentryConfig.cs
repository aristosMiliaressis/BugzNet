using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Configuration
{
    public class SentryConfig
    {
        public bool Enabled { get; set; }
        public string Dsn { get; set; }
        public bool CSPReporting { get; set; }
        public string CSPEndpoint { get; set; }
    }
}
