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

            WriteFile(_keyFileYandex, str);

            Console.WriteLine("Введите app_id и app_code для Here (через пробел):");
            str = Console.ReadLine();

            WriteFile(_keyFileHere, str);

            Console.WriteLine("Всего Доброго!");
            Console.ReadLine();
        }

        private static void WriteFile(string fileName, string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var s = ProtectedDataDPAPI.EncryptData(data);

                File.WriteAllText(fileName, s);
                Console.WriteLine("Ключ сформирован успешно");
                File.Delete("limit.dat");
            }
            else
            {
                Console.WriteLine("Ключ не обновлен");
            }
        }
    }
}
