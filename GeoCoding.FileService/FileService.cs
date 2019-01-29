using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeoCoding.FileService
{
    /// <summary>
    /// Класс для орагнизации работы с файлами, реализует IFileService
    /// </summary>
    public class FileService : IFileService
    {
        #region PrivateConst
        /// <summary>
        /// Фильтр типов расширения для файлов с данными
        /// </summary>
        private const string _filterForGetFile = "Файл - csv (*.csv)|*.csv|Файл - txt (*.txt)|*.txt|Все файлы (*.*)|*.*";
        /// <summary>
        /// Заголовок окна для выбора файла с данными
        /// </summary>
        private const string _titleFileGetDialog = "Выбрать файл с адресами объектов";
        /// <summary>
        /// Тип расширения для сохраняемого файла
        /// </summary>
        private const string _extensionFileForSave = ".csv";
        /// <summary>
        /// Фильтр типов расширения для сохраняемого файла
        /// </summary>
        private const string _filterForSaveFile = "Файл - csv (*.csv)|*.csv";
        /// <summary>
        /// Заголовок для окна выбора файла для сохранения
        /// </summary>
        private const string _titleFileSaveDialog = "Указать имя сохраняемого файла";
        /// <summary>
        /// Имя по умолчанию для сохранения файлов
        /// </summary>
        private const string _defaultNameFileForSave = "UpLoad";
        #endregion PrivateConst

        /// <summary>
        /// Метод выбора файла c данными
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами полное имя файла и ошибка</param>
        public void GetFile(Action<string, Exception> callback, string defaultFolder = "")
        {
            Exception error = null;
            string data = string.Empty;
            string a = Environment.CurrentDirectory;
            string defFolder = string.Empty;

          //  if(Directory.Exists(defaultFolder))
            //{
              defFolder = defaultFolder;
          //  }
          //  else
          //  {
            //  defFolder = Environment.CurrentDirectory;
            //}
            OpenFileDialog fd = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = _filterForGetFile,
                Title = _titleFileGetDialog,
                InitialDirectory = defFolder
            };

            if (fd.ShowDialog() == true)
            {
                data = fd.FileName;
            }

            callback(data, error);
        }

        /// <summary>
        /// Метод получения данных из файла
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами множество строк и ошибка</param>
        /// <param name="file">Полное имя файла</param>
        public void GetData(Action<IEnumerable<string>, Exception> callback, string file)
        {
            Exception error = null;
            List<string> data = null;

            if (File.Exists(file))
            {
                data = new List<string>();

                try
                {
                    using (StreamReader sr = new StreamReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8))
                    {
                        while (!sr.EndOfStream)
                        {
                            data.Add(sr.ReadLine());
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = ex;
                    data = null;
                }
            }
            else
            {
                error = new FileNotFoundException();
            }

            callback(data, error);
        }

        /// <summary>
        /// Метод для выбора имени(папки) файла для сохранения данных
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами имени файла и ошибка</param>
        /// <param name="defaultName">Имя для сохранения (необязательно)</param>
        public void SetFileForSave(Action<string, Exception> callback, string defaultName = "")
        {
            Exception error = null;
            string data = string.Empty;

            if (string.IsNullOrEmpty(defaultName))
            {
                defaultName = $"{DateTime.Now.ToString("yyyy_MM_dd")}_{_defaultNameFileForSave}";
            }

            SaveFileDialog fd = new SaveFileDialog()
            {
                Filter = _filterForSaveFile,
                AddExtension = true,
                DefaultExt = _extensionFileForSave,
                InitialDirectory = Environment.CurrentDirectory,
                Title = _titleFileSaveDialog,
                FileName = defaultName
            };

            if (fd.ShowDialog() == true)
            {
                data = fd.FileName;
            }

            callback(data, error);
        }

        /// <summary>
        /// Метод для сохранения данных в файл
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="data">Множество данных для записи</param>
        /// <param name="file">Файл куда записывать</param>
        public void SaveData(Action<Exception> callback, IEnumerable<string> data, string file)
        {
            Exception error = null;

            try
            {
                if (Directory.Exists(Path.GetDirectoryName(file)))
                {
                    using (StreamWriter sw = new StreamWriter(File.Create(file), Encoding.Default))
                    {
                        foreach (var item in data)
                        {
                            sw.WriteLine(item);
                        }
                    }
                }
                else
                {
                    error = new DirectoryNotFoundException();
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(error);
        }

        /// <summary>
        /// Метод для открытия папки
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="str">Путь к папке/файлу</param>
        public void OpenFolder(Action<Exception> callback, string str)
        {
            Exception error = null;

            if (string.IsNullOrEmpty(str))
            {
                error = new ArgumentNullException();
            }
            else
            {
                try
                {
                    if (File.Exists(str))
                    {
                        System.Diagnostics.Process.Start("explorer", @"/select, " + str);
                    }
                    else if (Directory.Exists(str))
                    {
                        System.Diagnostics.Process.Start(str);
                    }
                    else
                    {
                        var path = Path.GetDirectoryName(str);
                        if (Directory.Exists(path))
                        {
                            System.Diagnostics.Process.Start(path);
                        }
                        else
                        {
                            error = new DirectoryNotFoundException();
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            }

            callback(error);
        }

        /// <summary>
        /// Метод для создания папки
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="str">Путь к папке</param>
        void CreateFolder(Action<Exception> callback, string str)
        {
          Exception error= null;
          try
          {
            if(!Directory.Exists(str))
            {
                  Directory.CreateDirectory(str);
            }
          }
          catch (Exception ex)
          {
            error = ex;
          }

          callback(error);



        }
    }
}
