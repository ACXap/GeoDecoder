namespace GeoCoding
{
    /// <summary>
    /// Перечисление типов файлов, для формирования имени файла по умолчанию
    /// </summary>
    public enum NameFilesType
    {
        /// <summary>
        /// Для выходных файлов
        /// </summary>
        Output,
        /// <summary>
        /// Для файлов с ошибками
        /// </summary>
        Errors,
        /// <summary>
        /// Для временных файлов
        /// </summary>
        Temp,
        /// <summary>
        /// Для файлов статистики
        /// </summary>
        Statistics
    }
}