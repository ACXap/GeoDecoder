namespace GeoCoding
{
    /// <summary>
    /// Перечисление статусов состояния синхронизации
    /// </summary>
    public enum StatusSyncType
    {
        /// <summary>
        /// Не синхронизирован
        /// </summary>
        NotSync,
        /// <summary>
        /// Синхронизирован
        /// </summary>
        Sync,
        /// <summary>
        /// Обработан с ошибками
        /// </summary>
        Error,
        /// <summary>
        /// Обрабатывается в данный момент
        /// </summary>
        SyncProcessed
    }
}