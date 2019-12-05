// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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