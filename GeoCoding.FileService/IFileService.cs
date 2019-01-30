﻿using System;
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
        void OpenFolder(Action<Exception> callback, string str);

        /// <summary>
        /// Метод для создания папки
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="str">Путь к папке</param>
        void CreateFolder(Action<Exception> callback, string str);
    }
}