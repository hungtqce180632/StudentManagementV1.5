using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StudentManagementV1._5.Converters
{
    // Lớp BoolToActiveStatusConverter
    // + Tại sao cần sử dụng: Chuyển đổi trạng thái boolean thành văn bản trạng thái
    // + Được sử dụng từ binding trong XAML để hiển thị trạng thái hoạt động dưới dạng văn bản
    // + Chức năng chính: Chuyển true thành "Active" và false thành "Inactive"
    public class BoolToActiveStatusConverter : IValueConverter
    {
        // 1. Từ binding trong XAML, nhận vào giá trị boolean
        // 2. Kiểm tra xem giá trị có phải là boolean và là true không
        // 3. Trả về chuỗi "Active" nếu là true, ngược lại trả về "Inactive"
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isActive && isActive ? "Active" : "Inactive";
        }

        // 1. Phương thức chuyển đổi ngược
        // 2. Không thực hiện chuyển đổi ngược vì không cần thiết
        // 3. Ném ngoại lệ NotImplementedException
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Lớp BoolToStatusColorConverter
    // + Tại sao cần sử dụng: Chuyển đổi trạng thái boolean thành màu sắc
    // + Được sử dụng từ binding trong XAML để hiển thị trạng thái hoạt động dưới dạng màu sắc
    // + Chức năng chính: Chuyển true thành màu xanh và false thành màu đỏ
    public class BoolToStatusColorConverter : IValueConverter
    {
        // 1. Từ binding trong XAML, nhận vào giá trị boolean
        // 2. Kiểm tra xem giá trị có phải là boolean và là true không
        // 3. Trả về màu xanh nếu là true, ngược lại trả về màu đỏ
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isActive && isActive ? 
                new SolidColorBrush(Colors.Green) : 
                new SolidColorBrush(Colors.Red);
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
