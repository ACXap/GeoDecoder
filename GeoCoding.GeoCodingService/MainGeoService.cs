using System.Collections;
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
                new YandexGeoCodingService(),
                new YandexRusGisGeoCodingService(),
                new SputnikGeoCodingService(),
                new SputnikRusGisGeoCodingService(),
                new RusGisDemoGeoCodingService(),
                new Test.GeoCodingTest()
            }));
        /// <summary>
        /// Свойство для получения коллекции всех имен геосервисов
        /// </summary>
        public static ReadOnlyCollection<string> AllNameService => new ReadOnlyCollection<string>(AllService.Select(x => x.Name).ToList());
        /// <summary>
        /// Метод получения геокодера по имени, если не найден возвращает первый из коллекции
        /// </summary>
        /// <param name="name">Имя геосервиса</param>
        /// <returns>Геосервис</returns>
        public static IGeoCodingService GetServiceByName(string name)
        {
            return AllService.FirstOrDefault(x => x.Name == name) ?? AllService.FirstOrDefault();
        }
    }
}