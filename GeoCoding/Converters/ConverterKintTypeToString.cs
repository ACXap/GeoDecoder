using System;
using System.Globalization;
using System.Windows.Data;

namespace GeoCoding
{
    /// <summary>
    /// Класс конвертер для конвертирования KindType (вид объекта) в строковое представление
    /// </summary>
    public class ConverterKindTypeToString : IValueConverter
    {
        // Прямое конвертирование
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = string.Empty;
            if (value != null)
            {
                var k = (KindType)value;
                switch (k)
                {
                    case KindType.None:
                        str = "Негеокодировано";
                        break;

                    case KindType.House:
                        str = "Дом";
                        break;

                    case KindType.Street:
                        str = "Улица";
                        break;

                    case KindType.Metro:
                        str = "Станция метро";
                        break;

                    case KindType.District:
                        str = "Район города";
                        break;

                    case KindType.Locality:
                        str = "Населённый пункт";
                        break;

                    case KindType.Area:
                        str = "Район / область";
                        break;

                    case KindType.Province:
                        str = "Область";
                        break;

                    case KindType.Country:
                        str = "Страна";
                        break;

                    case KindType.Hydro:
                        str = "Река / озеро / ручей / водохранилище";
                        break;

                    case KindType.Rainway:
                        str = "Ж.Д.станция";
                        break;

                    case KindType.Route:
                        str = "Линия метро / шоссе / ж.д.линия";
                        break;

                    case KindType.Vegetation:
                        str = "Лес / парк / сад";
                        break;

                    case KindType.Airport:
                        str = "Аэропорт";
                        break;

                    case KindType.Other:
                        str = "Прочее";
                        break;

                    default:
                        str = "Что-то новое!!!";
                        break;
                }
            }
            return str;
        }

        // Обратное конвертирование
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}