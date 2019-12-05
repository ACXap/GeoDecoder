// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding.FormKey.Helpers
{
    public static class ProtectedDataDPAPI
    {
        /// <summary>
        /// Строка для поучения энтропии
        /// </summary>
        private static readonly string _entropyString = $"{Environment.MachineName}{Environment.UserName}";

        public static string EncryptData(string data)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    byte[] d = Encoding.UTF8.GetBytes(data);
                    byte[] crypted = ProtectedData.Protect(d, GetEntropy(), DataProtectionScope.CurrentUser);
                    result = Convert.ToBase64String(crypted);
                }
                catch
                {
                }
            }

            return result;
        }

        public static string DecryptData(string data)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    byte[] d = Convert.FromBase64String(data);
                    byte[] decrypted = ProtectedData.Unprotect(d, GetEntropy(), DataProtectionScope.CurrentUser);
                    result = Encoding.UTF8.GetString(decrypted);
                }
                catch 
                {
                }
            }

            return result;
        }

        /// <summary>
        /// Метод для получения энтропии
        /// </summary>
        /// <returns>Энтропию</returns>
        private static byte[] GetEntropy()
        {
            MD5 md5 = MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(_entropyString));
        }
    }
}