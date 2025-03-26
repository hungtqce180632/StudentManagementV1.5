using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StudentManagementV1._5.Converters
{
    // Lớp CountToVisibilityConverter
    // + Tại sao cần sử dụng: Chuyển đổi số lượng phần tử trong collection thành Visibility
    // + Lớp này được gọi từ binding trong XAML để hiển thị/ẩn UI dựa trên số lượng items
    // + Chức năng chính: Hiển thị UI element khi collection rỗng (có thể đảo ngược logic)
    public class CountToVisibilityConverter : IValueConverter
    {
        // 1. Từ binding trong XAML, nhận vào giá trị số lượng (count) của collection
        // 2. Xử lý chuyển đổi số lượng thành Visibility dựa trên tham số
        // 3. Trả về Visibility.Visible nếu count == parameter, ngược lại trả về Visibility.Collapsed
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Parse the count value
            if (value == null) return Visibility.Collapsed;
            
            int count;
            if (value is int)
                count = (int)value;
            else if (!int.TryParse(value.ToString(), out count))
                return Visibility.Collapsed;
            
            // Parse the parameter (threshold)
            int threshold = 0;
            if (parameter != null)
            {
                if (parameter is int)
                    threshold = (int)parameter;
                else
                    int.TryParse(parameter.ToString(), out threshold);
            }
            
            // If count equals threshold, return Visible; otherwise Collapsed
            return count == threshold ? Visibility.Visible : Visibility.Collapsed;
        }

        // 1. Phương thức chuyển đổi ngược
        // 2. Không cần thiết cho trường hợp này nên không triển khai
        // 3. Ném ngoại lệ NotImplementedException
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
