using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Exceptions
{
    public class StartupException : Exception
    {
        public StartupException()
        { }

        public StartupException(string message)
            : base(message)
        { }

        public StartupException(string message, Exception innerEx)
             : base(message, innerEx)
        { }
    }
}
