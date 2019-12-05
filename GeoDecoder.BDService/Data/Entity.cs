// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

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
        public Guid FiasGuid { get; set; }
    }
}