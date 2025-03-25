using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StudentManagementV1._5.Converters
{
    // Lớp StringToVisibilityConverter2
    // + Tại sao cần sử dụng: Lớp converter dự phòng hoặc tham khảo
    // + Đã được di chuyển vào file BooleanConverters.cs để hợp nhất
    // + Chức năng chính: Chuyển đổi chuỗi thành Visibility, nhưng không còn được sử dụng trực tiếp
    // This class has been moved to BooleanConverters.cs for consolidation
    // Keeping this file for reference but renaming the class to avoid conflicts
    public class StringToVisibilityConverter2 : IValueConverter
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
}
