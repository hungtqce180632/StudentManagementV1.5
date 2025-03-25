using System.Configuration;
using System.Data;
using System.Windows;
using StudentManagementV1._5.Converters;
using StudentManagementV1._5.Services;
using StudentManagementV1._5.ViewModels;
using StudentManagementV1._5.Views;

namespace StudentManagementV1._5;

/*
 * Lớp App
 * 
 * Tại sao sử dụng:
 * - Là điểm khởi đầu của ứng dụng WPF
 * - Quản lý vòng đời và tài nguyên chung của ứng dụng
 * 
 * Quan hệ với các lớp khác:
 * - Khởi tạo các converter toàn cục để sử dụng trong XAML
 * - Tạo và hiển thị MainWindow khi ứng dụng khởi động
 * 
 * Chức năng chính:
 * - Khởi tạo các tài nguyên chung của ứng dụng
 * - Xử lý sự kiện khởi động và thoát của ứng dụng
 */
public partial class App : Application
{
    // 1. Constructor của lớp App
    // 2. Khởi tạo các converter toàn cục để sử dụng trong XAML
    // 3. Được gọi khi ứng dụng bắt đầu khởi động
    public App()
    {
        // Add static converters to application resources
        Resources.Add("BooleanToVisibilityConverter", BooleanConverters.BooleanToVisibility);
        Resources.Add("InverseBooleanConverter", BooleanConverters.InverseBoolean);
        Resources.Add("InverseBooleanToVisibilityConverter", BooleanConverters.InverseBooleanToVisibility);
        Resources.Add("StringToVisibilityConverter", BooleanConverters.StringToVisibility);
    }

    // 1. Xử lý sự kiện khởi động của ứng dụng
    // 2. Tạo và hiển thị cửa sổ chính (MainWindow)
    // 3. Được gọi sau khi ứng dụng đã hoàn tất khởi động
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
    }
}

