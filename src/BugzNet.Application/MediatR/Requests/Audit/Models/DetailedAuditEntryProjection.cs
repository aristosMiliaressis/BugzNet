using System.Collections.Generic;

namespace BugzNet.Application.Requests.Audit.Models
{
    public class DetailedAuditEntryProjection
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public string Table { get; set; }
        public string StrPrimaryKey { get; set; }
        public string StartDate { get; set; }
        public Dictionary<string, object> Changes { get; set; }
        public string RequestPath { get; set; }
        public string EntityIdentifier { get; set; }
    }
}
