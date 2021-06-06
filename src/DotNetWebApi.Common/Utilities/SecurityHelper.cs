using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DotNetWebApi.Common.Utilities
{
    public static class SecurityHelper
    {
        public static string GetSha256Hash(string input)
        {
            //using (var sha256 = new SHA256CryptoServiceProvider())
            using var sha256 = SHA256.Create();
            var byteValue = Encoding.UTF8.GetBytes(input);
            var byteHash = sha256.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
            //return BitConverter.ToString(byteHash).Replace("-", "").ToLower();
        }

        public static string CreateRandomBase64String()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public static string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
