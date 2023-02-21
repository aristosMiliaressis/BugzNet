using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Configuration.Models
{
    public class WorkerConfig
    {
        public string Name { get; set; }
        public uint CallbackInterval { get; set; }
    }
}
