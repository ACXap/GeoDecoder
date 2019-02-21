using System;
using System.Globalization;
using System.Windows.Data;

namespace GeoCoding
{
    public class ConverterStatToProgress : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return ((double)value) / 100.0f;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}