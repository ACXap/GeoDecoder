using System.Collections.ObjectModel;

namespace GeoCoding.GeoCodingService
{
    public static class MainGeoService
    {
        private static ReadOnlyObservableCollection<IGeoCodingService> _allService;

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
    }
}