using Microsoft.Extensions.Configuration;
using BugzNet.Infrastructure.Configuration.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BugzNet.Infrastructure.SMS;

namespace BugzNet.Infrastructure.Configuration
{
    public class AppConfig
    {
        public string BasePath { get; set; }
        public string TimeZone { get; set; }
        public int AuthTimeOutMin { get; set; }
        public bool UseSwagger { get; set; }
        public bool UseMiniProfiler { get; set; }
        public string TokenSigningKey { get; set; }
        public string HMACSecret { get; set; }
        public string WebRequestStaticToken { get; set; }
        public SentryConfig Sentry { get; set; }
        public EmailSender EmailSender { get; set; }
        public SMSOptions SMSOptions { get; set; }
        public SmtpSettings SmtpSettings { get; set; }
        public List<ReportSettings> Reports { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
        public ProxySettings ProxySettings { get; set; }
        public ConnectionString DefaultConnection { get; set; }
        public List<WorkerConfig> Workers { get; set; }

        public AppConfig()
        {
            Sentry = new SentryConfig();
            EmailSender = new EmailSender();
            ProxySettings = new ProxySettings();
            ConnectionStrings = new Dictionary<string, string>();
            Reports = new List<ReportSettings>();
            Workers = new List<WorkerConfig>();
        }
    }
}
