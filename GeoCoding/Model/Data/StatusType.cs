namespace GeoCoding
{
  /// <summary>
  /// Перечисление статусов геокодирования
  /// </summary>
    public enum StatusType
    {
      /// <summary>
      /// Негеокодирован
      /// </summary>
        NotGeoCoding,
        /// <summary>
        /// Геокодирован без ошибок
        /// </summary>
        OK,
        /// <summary>
        /// Геокодирован с ошибками
        /// </summary>
        Error,
        /// <summary>
        /// Геокодируется в данный момент
        /// </summary>
        GeoCodingNow
    }
}
