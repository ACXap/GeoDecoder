namespace GeoCoding
{
  /// <summary>
  /// Класс для хранения статистики
  /// </summary>
    public class Statistics
    {
      /// <summary>
      /// Всего объектов в коллекции
      /// </summary>
        public int AllEntity { get; set; }
        /// <summary>
        /// Всего объектов со статусом "ОК"
        /// </summary>
        public int OK { get; set; }
        /// <summary>
        /// Всего объектов со статусом "Ошибка"
        /// </summary>
        public int Error { get; set; }
        /// <summary>
        /// Всего объектов со статусом "Негеокодирован"
        /// </summary>
        public int NotGeoCoding { get; set; }
        /// <summary>
        /// Всего объектов со статусом "Геокодируется сейчас"
        /// </summary>
        public int GeoCodingNow { get; set; }
        /// <summary>
        /// Всего объектов с типом объекта "Дом"
        /// </summary>
        public int House { get; set; }
        /// <summary>
        /// Всего объектов с качеством геокодирования "Точное геокодирование"
        /// </summary>
        public int Exact { get; set; }
        /// <summary>
        /// Время геокодирования в секундах
        /// </summary>
        public double TimeGeoCod { get; set;}
    }
}
