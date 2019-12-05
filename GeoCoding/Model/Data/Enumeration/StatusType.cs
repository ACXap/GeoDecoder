// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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