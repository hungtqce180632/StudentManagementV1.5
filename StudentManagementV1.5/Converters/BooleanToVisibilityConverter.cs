using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StudentManagementV1._5.Converters
{
    // Lớp BooleanToVisibilityConverter đã được bình luận
    // + Tại sao cần sử dụng: Chuyển đổi boolean thành Visibility
    // + Đã được di chuyển vào file BooleanConverters.cs
    // + Chức năng chính: Chuyển true thành Visibility.Visible và false thành Visibility.Collapsed
    //public class BooleanToVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value is bool boolValue && boolValue ? Visibility.Visible : Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value is Visibility visibility && visibility == Visibility.Visible;
    //    }
    //}
    
    // Lớp BooleanToVisibilityConverter2
    // + Tại sao cần sử dụng: Phiên bản thay thế của BooleanToVisibilityConverter
    // + Được sử dụng khi cần phiên bản thay thế của converter để tránh xung đột
    // + Chức năng chính: Chuyển true thành Visibility.Visible và false thành Visibility.Collapsed
    public class BooleanToVisibilityConverter2 : IValueConverter
    {
        // 1. Từ binding trong XAML, nhận vào giá trị boolean
        // 2. Kiểm tra xem giá trị có phải là boolean và là true không
        // 3. Trả về Visibility.Visible nếu là true, ngược lại trả về Visibility.Collapsed
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue && boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        // 1. Từ binding ngược (two-way binding) trong XAML, nhận giá trị là Visibility
        // 2. Kiểm tra xem giá trị có phải là Visibility và là Visibility.Visible không
        // 3. Trả về true nếu là Visibility.Visible, ngược lại trả về false
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }
}
