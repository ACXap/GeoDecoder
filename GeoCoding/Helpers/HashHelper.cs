// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Security.Cryptography;
using System.Text;

namespace GeoCoding.Helpers
{
    public static class HashHelper
    {
        private const string _salt = "Orpon";
        public static string HashString(string data)
        {
            using SHA512 sha512Hash = SHA512.Create();
            var bytes = Encoding.UTF8.GetBytes($"{data}_{_salt}");
            var hashBytes = sha512Hash.ComputeHash(bytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            return hash;
        }

        public static bool VerifyString(string data, string hash)
        {
            return HashString(data) == hash;
        }
    }
}