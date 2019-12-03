using System;
using System.Security.Cryptography;
using System.Text;

namespace GeoCoding.Helpers
{
    public static class HashHelper
    {
        private static string _salt = "Orpon";
        public static string HashString(string data)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                var bytes = Encoding.UTF8.GetBytes($"{data}_{_salt}");
                var hashBytes = sha512Hash.ComputeHash(bytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash;
            }
        }

        public static bool VerifyString(string data, string hash)
        {
            return HashString(data) == hash;
        }
    }
}