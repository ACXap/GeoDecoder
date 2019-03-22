using System;
using System.Collections.Generic;

namespace GeoCoding.FileService
{
    /// <summary>
    /// Интерфейс для описания работы с файлами
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Метод выбора файла c данными
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами полное имя файла и ошибка</param>
        /// <param name="defaultFolder">Имя папки по умолчанию для открытия</param>
        void GetFile(Action<string, Exception> callback, string defaultFolder = "");

        /// <summary>
        /// Метод выбора файлов с данными
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами список файлов и ошибка</param>
        /// <param name="defaultFolder">Имя папки по умолчанию для открытия</param>
        void GetFiles(Action<IEnumerable<string>, Exception> callback, string defaultFolder = "");

        /// <summary>
        /// Метод для подсчета строк в файле
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами количество записей и ошибка</param>
        /// <param name="fileName">Имя файла</param>
        void GetCountRecord(Action<int, Exception> callback, string fileName);

        /// <summary>
        /// Метод получения данных из файла
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами множество строк и ошибка</param>
        /// <param name="file">Полное имя файла</param>
        void GetData(Action<IEnumerable<string>, Exception> callback, string file);

        /// <summary>
        /// Метод для выбора имени(папки) файла для сохранения данных
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами имени файла и ошибка</param>
        void SetFileForSave(Action<string, Exception> callback, string defaultName = "");

        /// <summary>
        /// Метод для сохранения данных в файл
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="data">Множество данных для записи</param>
        /// <param name="file">Файл куда записывать</param>
        void SaveData(Action<Exception> callback, IEnumerable<string> data, string file);

        /// <summary>
        /// Метод для открытия папки
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="str">Путь к папке, к файлу</param>
        void OpenFolder(Action<Exception> callback, string path);

        /// <summary>
        /// Метод для создания папки
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="str">Путь к папке</param>
        void CreateFolder(Action<Exception> callback, string path);

        /// <summary>
        /// Метод для проверки наличия файла
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром наличие файла и ошибка</param>
        /// <param name="file">Имя файла</param>
        void FileExists(Action<bool, Exception> callback, string file);

        /// <summary>
        /// Метод для добавления данных в файл
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="data">Данные для добавления</param>
        /// <param name="file">Имя файла</param>
        void AppendData(Action<Exception> callback, IEnumerable<string> data, string file);

        void GetByteFromFile(Action<byte[], Exception> callback, string file);
    }
}