namespace GeoCoding.GeoCodingService
{
    /// <summary>
    /// Класс для хранения объекта геокодирования
    /// </summary>
    public class GeoCod
    {
        /// <summary>
        /// Адрес
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Вид объекта
        /// </summary>
        public string Kind { get; set; }
        /// <summary>
        /// Качество геокординат
        /// </summary>
        public string Precision { get; set; }
        /// <summary>
        /// Долгота
        /// </summary>
        public string Longitude { get; set; }
        /// <summary>
        /// Широта
        /// </summary>
        public string Latitude { get; set; }
        /// <summary>
        /// Точность по сегментам
        /// </summary>
        public string MatchQuality { get; set; }
    }
}