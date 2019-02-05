namespace GeoCoding.BDService
{
    /// <summary>
    /// Класс для хранения объекта из базы данных
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// OrponId - орпон айди объекта
        /// </summary>
        public int OrponId { get; set; }
        /// <summary>
        /// Address - адрес объекта
        /// </summary>
        public string Address { get; set; }
    }
}