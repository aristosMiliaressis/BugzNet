using System;

namespace BugzNet.Core.Utilities
{
    public static class OtpGenerator
    {
        public static string Generate()
        {
            var rng = new Random((int)(DateTime.UtcNow.Ticks / 10000 & int.MaxValue));
            return rng.Next(1000000).ToString().PadLeft(6, '0');
        }
    }
}