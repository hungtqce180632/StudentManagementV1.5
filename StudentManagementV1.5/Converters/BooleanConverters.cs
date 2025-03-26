using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StudentManagementV1._5.Converters
{
    // Lớp BooleanToVisibilityConverter
    // + Tại sao cần sử dụng: Dùng để chuyển đổi giá trị Boolean thành Visibility trong XAML
    // + Lớp này được gọi từ các phần binding trong XAML và trả về Visibility để điều khiển hiển thị của UI elements
    // + Chức năng chính: Khi giá trị là true thì hiển thị (Visible), false thì ẩn (Collapsed)
    public class BooleanToVisibilityConverter : IValueConverter
    {
        // 1. Từ binding trong XAML, nhận vào giá trị boolean
        // 2. Xử lý chuyển đổi từ boolean sang Visibility
        // 3. Trả về Visibility.Visible nếu giá trị là true, ngược lại trả về Visibility.Collapsed
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        // 1. Từ binding ngược (two-way binding) trong XAML, nhận giá trị là Visibility
        // 2. Xử lý chuyển đổi từ Visibility thành boolean
        // 3. Trả về true nếu giá trị là Visible, ngược lại trả về false
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }

    // Lớp InverseBooleanToVisibilityConverter
    // + Tại sao cần sử dụng: Dùng để chuyển đổi giá trị Boolean ngược thành Visibility trong XAML
    // + Lớp này được gọi từ các phần binding trong XAML và trả về Visibility ngược với giá trị Boolean
    // + Chức năng chính: Khi giá trị là false thì hiển thị (Visible), true thì ẩn (Collapsed)
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        // 1. Từ binding trong XAML, nhận vào giá trị boolean
        // 2. Xử lý chuyển đổi ngược từ boolean sang Visibility
        // 3. Trả về Visibility.Visible nếu giá trị là false, ngược lại trả về Visibility.Collapsed
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool && !(bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        // 1. Từ binding ngược (two-way binding) trong XAML, nhận giá trị là Visibility
        // 2. Xử lý chuyển đổi ngược từ Visibility thành boolean
        // 3. Trả về true nếu giá trị là Collapsed, ngược lại trả về false
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility && (Visibility)value == Visibility.Collapsed;
        }
    }

    // Lớp InverseBooleanConverter
    // + Tại sao cần sử dụng: Dùng để đảo ngược giá trị Boolean trong XAML binding
    // + Lớp này được gọi từ các phần binding cần đảo ngược logic boolean
    // + Chức năng chính: Đảo ngược giá trị boolean (true thành false và ngược lại)
    public class InverseBooleanConverter : IValueConverter
    {
        // 1. Từ binding trong XAML, nhận vào giá trị boolean
        // 2. Xử lý đảo ngược giá trị boolean
        // 3. Trả về phủ định của giá trị boolean đầu vào
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && !(bool)value;
        }

        // 1. Từ binding ngược (two-way binding) trong XAML, nhận vào giá trị boolean
        // 2. Xử lý đảo ngược giá trị boolean
        // 3. Trả về phủ định của giá trị boolean đầu vào
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && !(bool)value;
        }
    }

    // Lớp StringToVisibilityConverter
    // + Tại sao cần sử dụng: Dùng để chuyển đổi chuỗi thành Visibility trong XAML
    // + Lớp này được gọi từ các phần binding chuỗi cần điều khiển hiển thị UI
    // + Chức năng chính: Hiển thị UI khi chuỗi không rỗng, ẩn khi chuỗi rỗng
    public class StringToVisibilityConverter : IValueConverter
    {
        // 1. Từ binding trong XAML, nhận vào giá trị chuỗi
        // 2. Kiểm tra chuỗi có rỗng hay không
        // 3. Trả về Visibility.Collapsed nếu chuỗi rỗng, ngược lại trả về Visibility.Visible
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        // 1. Phương thức chuyển đổi ngược
        // 2. Không thực hiện chuyển đổi ngược vì không cần thiết
        // 3. Ném ngoại lệ NotImplementedException
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Lớp tĩnh BooleanConverters
    // + Tại sao cần sử dụng: Cung cấp các instance của converter để sử dụng trong XAML
    // + Lớp này được gọi trực tiếp từ XAML thông qua StaticResource
    // + Chức năng chính: Cung cấp các instance sẵn có của converter để sử dụng trong XAML
    /// <summary>
    /// Static class that provides converter instances for use in XAML
    /// </summary>
    public static class BooleanConverters
    {
        // 1. Khởi tạo instance của BooleanToVisibilityConverter
        // 2. Cung cấp như một thuộc tính tĩnh
        // 3. Có thể truy cập trực tiếp từ XAML
        public static readonly BooleanToVisibilityConverter BooleanToVisibility = new BooleanToVisibilityConverter();
        
        // 1. Khởi tạo instance của InverseBooleanToVisibilityConverter
        // 2. Cung cấp như một thuộc tính tĩnh
        // 3. Có thể truy cập trực tiếp từ XAML
        public static readonly InverseBooleanToVisibilityConverter InverseBooleanToVisibility = new InverseBooleanToVisibilityConverter();
        
        // 1. Khởi tạo instance của InverseBooleanConverter
        // 2. Cung cấp như một thuộc tính tĩnh
        // 3. Có thể truy cập trực tiếp từ XAML
        public static readonly InverseBooleanConverter InverseBoolean = new InverseBooleanConverter();
        
        // 1. Khởi tạo instance của StringToVisibilityConverter
        // 2. Cung cấp như một thuộc tính tĩnh
        // 3. Có thể truy cập trực tiếp từ XAML
        public static readonly StringToVisibilityConverter StringToVisibility = new StringToVisibilityConverter();
        
        // 1. Khởi tạo instance của CountToVisibilityConverter
        // 2. Cung cấp như một thuộc tính tĩnh
        // 3. Có thể truy cập trực tiếp từ XAML
        public static readonly CountToVisibilityConverter CountToVisibility = new CountToVisibilityConverter();
    }
}
