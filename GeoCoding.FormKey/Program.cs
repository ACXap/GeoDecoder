using GeoCoding.FormKey.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding.FormKey
{
    class Program
    {
        private const string _keyFile = "key";
        static void Main(string[] args)
        {
            Console.WriteLine("Введите ключ:");
            var str = Console.ReadLine();

            var s = ProtectedDataDPAPI.EncryptData(str);

            File.WriteAllText(_keyFile, s);
            Console.WriteLine("Ключ сформирован успешно");
            Console.ReadLine();
        }
    }
}
