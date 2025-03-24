using System;
using System.Globalization;
using System.Windows.Data;

namespace StudentManagementV1._5.Converters
{
    //public class InverseBooleanConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return !(value is bool boolValue && boolValue);
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return !(value is bool boolValue && boolValue);
    //    }
    //}
    public class InverseBooleanConverterDuplicate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is bool boolValue && boolValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is bool boolValue && boolValue);
        }
    }
}
