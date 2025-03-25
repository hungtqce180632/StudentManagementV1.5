using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StudentManagementV1._5.Converters
{
    // Lớp InverseBooleanToVisibilityConverterV2
    // + Tại sao cần sử dụng: Phiên bản thay thế hoặc cập nhật của InverseBooleanToVisibilityConverter
    // + Được sử dụng khi cần phiên bản khác của converter để tránh xung đột với phiên bản chính
    // + Chức năng chính: Đảo ngược giá trị boolean sang Visibility với logic khác
    // Renamed the class to avoid conflict
    public class InverseBooleanToVisibilityConverterV2 : IValueConverter
    {
        // 1. Từ binding trong XAML, nhận vào giá trị boolean
        // 2. Xử lý chuyển đổi giá trị boolean sang Visibility với logic đảo ngược
        // 3. Trả về Visibility.Collapsed nếu giá trị là true, ngược lại trả về Visibility.Visible
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue && boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        // 1. Từ binding ngược (two-way binding) trong XAML, nhận giá trị là Visibility
        // 2. Xử lý chuyển đổi Visibility thành boolean với logic đảo ngược
        // 3. Trả về true nếu giá trị là Collapsed, ngược lại trả về false
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Collapsed;
        }
    }
}
