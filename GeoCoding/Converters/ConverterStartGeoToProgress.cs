using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shell;

namespace GeoCoding
{
    public class ConverterStartGeoToProgress : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if ((bool)value)
                {
                    return TaskbarItemProgressState.Normal;
                }
            }
            return TaskbarItemProgressState.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}