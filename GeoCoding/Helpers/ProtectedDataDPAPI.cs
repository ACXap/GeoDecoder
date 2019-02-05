using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding.Helpers
{
    public static class ProtectedDataDPAPI
    {
        public static void EncryptData(Action<string, Exception> callback, string data)
        {
            Exception error = null;
            string result = string.Empty;

            if (string.IsNullOrEmpty(data))
            {
                error = new ArgumentNullException();
            }
            else
            {
                try
                {
                    byte[] entropy = GetEntropy(Environment.MachineName + Environment.UserName);
                    byte[] d = Encoding.UTF8.GetBytes(data);
                    byte[] crypted = ProtectedData.Protect(d, entropy, DataProtectionScope.CurrentUser);
                    result = Convert.ToBase64String(crypted);
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            }

            callback(result, error);
        }

        public static void DecryptData(Action<string, Exception> callback, string data)
        {
            Exception error = null;
            string result = string.Empty;

            if (string.IsNullOrEmpty(data))
            {
                error = new ArgumentNullException();
            }
            else
            {
                try
                {
                    byte[] d = Convert.FromBase64String(data);
                    byte[] entropy = GetEntropy(Environment.MachineName + Environment.UserName);
                    byte[] decrypted = ProtectedData.Unprotect(d, entropy, DataProtectionScope.CurrentUser);
                    result = Encoding.UTF8.GetString(decrypted);
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            }

            callback(result, error);
        }

        private static byte[] GetEntropy(string EntropyString)
        {

            MD5 md5 = MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(EntropyString));
        }
    }
}
