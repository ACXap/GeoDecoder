﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GeoCoding.GeoCodingService
{
    public static class MainGeoService
    {
        /// <summary>
        /// Поле для хранения коллекции геосервисов
        /// </summary>
        private static ReadOnlyCollection<IGeoCodingService> _allService;
        /// <summary>
        /// Свойство для получения коллекции всех геокодеров
        /// </summary>
        public static ReadOnlyCollection<IGeoCodingService> AllService =>
            _allService ?? (_allService = new ReadOnlyCollection<IGeoCodingService>(new List<IGeoCodingService>()
            {
              new YandexPayGeoCodingService(),
              new HereGeoCodingService(),
                //  new YandexGeoCodingService(),
               // new YandexRusGisGeoCodingService(),
                //new SputnikGeoCodingService(),
               // new SputnikRusGisGeoCodingService(),
              //  new RusGisDemoGeoCodingService(),
              //  new RusGisGeoCodingService(),
             //   new HereRusGisGeoCodingService(),
              //  new Test.GeoCodingTest()
            }));
        /// <summary>
        /// Свойство для получения коллекции всех имен геосервисов
        /// </summary>
        public static ReadOnlyCollection<string> AllNameService => new ReadOnlyCollection<string>(AllService.Select(x => x.Name).ToList());
        /// <summary>
        /// Метод получения геокодера по имени
        /// </summary>
        /// <param name="name">Имя геосервиса</param>
        /// <param name="key">Апи - ключ</param>
        /// <returns>Геосервис</returns>
        public static IGeoCodingService GetServiceByName(string name, string key)
        {
            var g = AllService.FirstOrDefault(x => x.Name == name);
            g.SetKeyApi(key);

            return g;
        }
    }
}