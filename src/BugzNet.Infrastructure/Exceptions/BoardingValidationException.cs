using BugzNet.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Exceptions
{
    public class BoardingValidationException : RequestValidationException
    {
        public BoardingValidationException(string message)
            : base(message)
        {
        }

        public BoardingValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
