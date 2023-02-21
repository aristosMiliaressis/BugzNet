using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugzNet.Web.Extensions
{
    public static class ValidationResultExtensions
    {
        public static string ToErrorMessage(this ValidationResult result)
        {
            return string.Join(Environment.NewLine, result.Errors.Select(e => e.ErrorMessage));
        }
    }
}
