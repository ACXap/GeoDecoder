// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Globalization;
using System.Windows.Data;

namespace GeoCoding
{
    /// <summary>
    /// Класс конвертер для конвертирования PrecisionType (качество геокодирования) в строковое представление
    /// </summary>
    public class ConverterPrecisionTypeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = string.Empty;
            if (value != null)
            {
                var pt = (PrecisionType)value;

                switch (pt)
                {
                    case PrecisionType.None:
                        str = "Негеокодировано";
                        break;

                    case PrecisionType.Exact:
                        str = "Точное совпадение";
                        break;

                    case PrecisionType.Number:
                        str = "Найден дом с номером, но другой корпус/строение";
                        break;

                    case PrecisionType.Near:
                        str = "Найден дом с номером, близким к запрошенному";
                        break;

                    case PrecisionType.Range:
                        str = "Найдены приблизительные координаты запрашиваемого дома";
                        break;

                    case PrecisionType.Street:
                        str = "Найдена только улица";
                        break;

                    case PrecisionType.Other:
                        str = "Найден только населённый пункт";
                        break;

                    default:
                        str = "Что-то новое!!!";
                        break;
                }
            }
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}