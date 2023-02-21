using System;
using System.Collections.Generic;
using System.Text;

namespace BugzNet.Infrastructure.Configuration
{
    public class ReportSettings
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public ExportSettings[] Exports { get; set; }
        public string[] Recipients { get; set; }
        public string CronExpression { get; set; }
        
    }

    public class ExportSettings
    {
        public string Name { get; set; }
        public string FileNameTemplate { get; set; }
        public string ExtraData { get; set; }
    }
}
