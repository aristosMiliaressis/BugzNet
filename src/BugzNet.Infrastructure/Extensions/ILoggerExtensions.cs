using Microsoft.Extensions.Logging;
using BugzNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Extensions
{
    public static class ILoggerExtensions
    {
        /// <summary>
        /// Recursivly logs an exception.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        /// <param name="critical">if true, it will send sentry alert</param>
        public static void LogException(this ILogger logger, Exception ex, bool critical = false)
        {
            var exceprionInfo = $"{ex.ToRecursiveStackTrace()}{Environment.NewLine}";
            if (critical) logger.LogCritical(exceprionInfo, ex);
            else logger.LogError(exceprionInfo, ex);
        }
    }
}
