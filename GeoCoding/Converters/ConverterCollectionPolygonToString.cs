// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace GeoCoding
{
    public class ConverterCollectionPolygonToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value!=null)
            {
                if (value is List<double> data && data.Count==4)
                {
                    return $"МинДолгота: {data[0]}, МинШирота: {data[1]}, МаксДолгота: {data[2]}, МаксШирота: {data[3]}";
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}