using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace BugzNet.Infrastructure.Validators
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class MatchAnyRegExAttribute : ValidationAttribute
    {
        public Regex[] Expressions { get; set; }

        public MatchAnyRegExAttribute(params string[] expressions)
        {
            Expressions = expressions.Select(exp => new Regex(exp)).ToArray();
        }

        public override bool IsValid(object value)
        {
            if (!(value == null))
                return Expressions.Any(regex => regex.IsMatch(value.ToString()));
            return true;
        }
    }
}
