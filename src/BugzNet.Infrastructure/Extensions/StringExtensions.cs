using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string ToHumanFriendlySpacing(this string text)
        {
            string[] split = Regex.Split(text, @"(?<!^)(?=[A-Z])");// /([a-z\xE0-\xFF])([A-Z\xC0\xDF])/g

            return split.All(p => p.Length == 1) ? string.Join("", split) : string.Join(" ", split);
        }

        public static int ToIntOrZero(this string value)
        {
            var result = int.TryParse(value, out int integer);

            return result ? integer : 0;
        }

        public static string ToSpecificLen(this string input, int desiredLen)
        {
            return input.Length > desiredLen ? input.Substring(0, desiredLen) : input;
        }
    }
}
