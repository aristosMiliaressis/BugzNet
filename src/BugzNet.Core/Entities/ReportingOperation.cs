using BugzNet.Core.SharedKernel;
using System;

namespace BugzNet.Core.Entities
{
    public class ReportingOperation : BaseEntity<int>
    {
        public string Name { get; private set; }

        public DateTime LastOccurrence { get; private set; }
        public DateTime NextOccurrence { get; private set; }

        private ReportingOperation() { }

        public ReportingOperation(string uid)
        {
            Name = uid;
        }

        public void SetLastOccurrence(DateTime time)
        {
            LastOccurrence = time;
        }

        public void SetNextOccurrence(DateTime time)
        {
            NextOccurrence = time;
        }
    }
}
