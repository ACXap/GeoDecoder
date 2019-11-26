using GeoCoding.Model.Data;
using System;
using System.Security.Cryptography;
using System.Text;

namespace GeoCoding.Helpers
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

        /// <summary>
        /// Метод для дешифровки данных
        /// </summary>
        public static EntityResult<string> DecryptData(string data)
        {
            EntityResult<string> result = new EntityResult<string>();

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    byte[] d = Convert.FromBase64String(data);
                    byte[] decrypted = ProtectedData.Unprotect(d, GetEntropy(), DataProtectionScope.CurrentUser);
                    result.Object = Encoding.UTF8.GetString(decrypted);
                    result.Successfully = true;
                }
                catch (Exception ex)
                {
                    result.Successfully = false;
                    result.Error = ex;
                }
            }

            return result;
        }

        /// <summary>
        /// Метод для шифрования данных
        /// </summary>
        public static EntityResult<string> EncryptData(string data)
        {
            EntityResult<string> result = new EntityResult<string>();

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    byte[] d = Encoding.UTF8.GetBytes(data);
                    byte[] crypted = ProtectedData.Protect(d, GetEntropy(), DataProtectionScope.CurrentUser);
                    result.Object = Convert.ToBase64String(crypted);
                    result.Successfully = true;
                }
                catch (Exception ex)
                {
                    result.Error = ex;
                    result.Successfully = false;
                }
            }

            return result;
        }
    }
}