// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Security.Cryptography;
using System.Text;

namespace GeoCoding.GeoCodingService.Helpers
{
    /// <summary>
    /// Класс для реализации шифрования и дешифровки данных
    /// </summary>
    public static class ProtectedDataDPAPI
    {
        /// <summary>
        /// Строка для поучения энтропии
        /// </summary>
        private static readonly string _entropyString = $"{Environment.MachineName}{Environment.UserName}";

        /// <summary>
        /// Метод для шифрования данных
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром: строка, ошибка</param>
        /// <param name="data">Данные для шифрования</param>
        public static void EncryptData(Action<string, Exception> callback, string data)
        {
            Exception error = null;
            string result = string.Empty;

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    byte[] d = Encoding.UTF8.GetBytes(data);
                    byte[] crypted = ProtectedData.Protect(d, GetEntropy(), DataProtectionScope.CurrentUser);
                    result = Convert.ToBase64String(crypted);
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            }

            callback(result, error);
        }

        /// <summary>
        /// Метод для дешифровки данных
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами: строка, ошибка</param>
        /// <param name="data">Данные для дешифровки</param>
        public static void DecryptData(Action<string, Exception> callback, string data)
        {
            Exception error = null;
            string result = string.Empty;

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    byte[] d = Convert.FromBase64String(data);
                    byte[] decrypted = ProtectedData.Unprotect(d, GetEntropy(), DataProtectionScope.CurrentUser);
                    result = Encoding.UTF8.GetString(decrypted);
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            }

            callback(result, error);
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
