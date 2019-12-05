﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace GeoCoding
{
    /// <summary>
    /// Описывает точность соответствия запроса и результата.
    /// </summary>
    public enum PrecisionType
    {
        /// <summary>
        /// Геокодирование не проводилось
        /// </summary>
        None,
        /// <summary>
        /// Запрос: (27 строение 1) Ответ:(27с1) - Найден дом с указанным номером дома.
        /// </summary>
        Exact,
        /// <summary>
        /// Запрос: (31 корпус 4) Ответ:(31к2) - Найден дом с указанным номером, но с другим номером строения или корпуса.
        /// </summary>
        Number,
        /// <summary>
        /// Запрос: (16/3) Ответ:(18) - Найден дом с номером, близким к запрошенному.
        /// </summary>
        Near,
        /// <summary>
        /// Запрос: (12) Ответ:(12) - Найдены приблизительные координаты запрашиваемого дома.
        /// </summary>
        Range,
        /// <summary>
        /// Запрос: (18) Ответ:(	– ) - Найдена только улица.
        /// </summary>
        Street,
        /// <summary>
        /// Запрос: (22) Ответ:(	– ) - Не найдена улица, но найден, например, посёлок, район и т.п.
        /// </summary>
        Other
    }
}