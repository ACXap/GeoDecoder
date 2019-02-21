using System.Collections.ObjectModel;
using System.Linq;

namespace GeoCoding.GeoCodingService
{
    public static class MainGeoService
    {
        /// <summary>
        /// Поле для хранения коллекции геосервисов
        /// </summary>
        private static ReadOnlyObservableCollection<IGeoCodingService> _allService;
        /// <summary>
        /// Свойство для получения коллекции всех геокодеров
        /// </summary>
        public static ReadOnlyObservableCollection<IGeoCodingService> AllService =>
            _allService ?? (_allService = new ReadOnlyObservableCollection<IGeoCodingService>(new ObservableCollection<IGeoCodingService>()
            {
                new YandexGeoCodingService(),
                new YandexRusGisGeoCodingService(),
                new SputnikGeoCodingService(),
                new SputnikRusGisGeoCodingService(),
                new RusGisDemoGeoCodingService(),
                new Test.GeoCodingTest()
            }));
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