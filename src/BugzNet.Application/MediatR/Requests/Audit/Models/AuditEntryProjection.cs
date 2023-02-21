using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugzNet.Application.Requests.Audit.Models
{
    public class AuditEntryProjection
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public string Table { get; set; }
        public string StrPrimaryKey { get; set; }
        public string At { get; set; }
        public string TraceIdentifier { get; set; }
    }
}
