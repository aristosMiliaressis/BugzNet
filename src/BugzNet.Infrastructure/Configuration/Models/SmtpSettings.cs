using BugzNet.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BugzNet.Infrastructure.Configuration
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
