namespace GeoCoding
{
    /// <summary>
    /// Перечисление статусов работы
    /// </summary>
    public enum StatusType
    {
        /// <summary>
        /// Не обработан
        /// </summary>
        NotProcessed,
        /// <summary>
        /// Обработан без ошибок
        /// </summary>
        OK,
        /// <summary>
        /// Обработан с ошибками
        /// </summary>
        Error,
        /// <summary>
        /// Обрабатывается в данный момент
        /// </summary>
        Processed
    }
}