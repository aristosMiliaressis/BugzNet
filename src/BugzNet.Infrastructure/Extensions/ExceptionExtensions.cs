using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToRecursiveStackTrace(this Exception ex)
        {
            string exceptionInfo = $"--> {ex.GetType().Name}: {ex.Message}{Environment.NewLine}{ex.StackTrace}";
            if (ex.InnerException == null)
                return exceptionInfo;
            else
                return exceptionInfo + Environment.NewLine + ex.InnerException.ToRecursiveStackTrace();
        }
    }
}
