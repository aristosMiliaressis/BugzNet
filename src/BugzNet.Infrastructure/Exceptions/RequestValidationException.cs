using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Exceptions
{
    public class RequestValidationException : Exception
    {
        public RequestValidationException(string message)
            : base(message)
        {
        }

        public RequestValidationException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }
    }
}
