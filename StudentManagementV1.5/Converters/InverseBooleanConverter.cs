using System;
using System.Globalization;
using System.Windows.Data;

namespace StudentManagementV1._5.Converters
{
    // Lớp InverseBooleanConverter đã được bình luận
    // + Tại sao cần sử dụng: Đảo ngược giá trị Boolean
    // + Đã được di chuyển vào file BooleanConverters.cs
    // + Chức năng chính: Đảo ngược giá trị boolean
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
    
    // Lớp InverseBooleanConverterDuplicate
    // + Tại sao cần sử dụng: Phiên bản sao chép của InverseBooleanConverter với logic tối ưu hơn
    // + Được sử dụng khi cần phiên bản thay thế của converter để thử nghiệm hoặc tránh xung đột
    // + Chức năng chính: Đảo ngược giá trị boolean với cách viết khác
    public class InverseBooleanConverterDuplicate : IValueConverter
    {
        // 1. Từ binding trong XAML, nhận vào giá trị object
        // 2. Kiểm tra xem giá trị có phải là boolean và là true không
        // 3. Trả về phủ định của kết quả kiểm tra
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is bool boolValue && boolValue);
        }

        // 1. Từ binding ngược (two-way binding) trong XAML, nhận vào giá trị object
        // 2. Kiểm tra xem giá trị có phải là boolean và là true không
        // 3. Trả về phủ định của kết quả kiểm tra
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is bool boolValue && boolValue);
        }
    }
}
