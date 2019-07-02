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
        private const string _keyFileHere = "keyHere";
        private const string _keyFileYandex = "keyYandex";
        static void Main(string[] args)
        {
            Console.WriteLine("Введите ключ для Yandex:");
            var str = Console.ReadLine();

            var s = ProtectedDataDPAPI.EncryptData(str);

            File.WriteAllText(_keyFileYandex, s);
            Console.WriteLine("Ключ сформирован успешно");

            Console.WriteLine("Введите app_id и app_code для Here (через пробел):");
            str = Console.ReadLine();

            s = ProtectedDataDPAPI.EncryptData(str);

            File.WriteAllText(_keyFileHere, s);
            Console.WriteLine("Ключ сформирован успешно");
            Console.ReadLine();
        }
    }
}
