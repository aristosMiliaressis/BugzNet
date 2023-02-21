using BugzNet.Infrastructure.Configuration;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using System.Text;

namespace BugzNet.Infrastructure
{
    public static class CryptoUtility
    {
        private static IDataProtectionProvider _dataProtectionProvider = DataProtectionProvider.Create("BugzNet");
        private static IDataProtector _dataProtector = _dataProtectionProvider.CreateProtector("config");

        public static string Decypt(string cipherText)
        {
            return _dataProtector.Unprotect(cipherText);
        }

        public static string CalculateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
