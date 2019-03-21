namespace GeoCoding
{
    public enum StatusCompareType
    {
        /// <summary>
        /// Не сравнивался еще
        /// </summary>
        NotCompare,
        /// <summary>
        /// Был сравнен
        /// </summary>
        OK,
        /// <summary>
        /// Сравнен с ошибкой
        /// </summary>
        Error,
        /// <summary>
        /// Сравнивается в данный момент
        /// </summary>
        CompareNow
    }
}