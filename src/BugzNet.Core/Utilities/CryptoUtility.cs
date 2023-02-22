namespace BugzNet.Core.Utilities;

using System;
using System.Security.Cryptography;
using System.Text;

public static class CryptoUtility
{
    public static string Sign(string text, string key)
    {
        using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToBase64String(hash);
        }
    }
}