// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shell;

namespace GeoCoding
{
    /// <summary>
    /// Класс конвертер запущенного процесса в состояние индикатора выполнения на панели задач
    /// </summary>
    public class ConverterStatGeoToProgressState : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null) && (bool)value ? TaskbarItemProgressState.Normal : TaskbarItemProgressState.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}