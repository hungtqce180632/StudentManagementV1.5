using StudentManagementV1._5.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Add this missing namespace for CommandManager

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp LoginView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện đăng nhập cho người dùng
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng LoginViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này khi bắt đầu ứng dụng
     * 
     * Chức năng chính:
     * - Hiển thị form đăng nhập với tên đăng nhập và mật khẩu
     * - Xử lý binding đặc biệt cho PasswordBox (vì lý do bảo mật)
     * - Liên kết với chức năng đặt lại mật khẩu
     */
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            // Connect to loaded event to set up password binding
            Loaded += LoginView_Loaded;
        }

        // 1. Phương thức xử lý sự kiện Loaded của control
        // 2. Thiết lập binding cho PasswordBox và xử lý thay đổi mật khẩu
        // 3. Đảm bảo CommandManager kiểm tra lại điều kiện thực thi lệnh
        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                // Handle password box changes
                if (PasswordBox != null)
                {
                    PasswordBox.PasswordChanged += (s, args) =>
                    {
                        viewModel.Password = PasswordBox.Password;
                    };
                }
                
                // Also ensure command can execute is checked
                viewModel.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(LoginViewModel.Password) ||
                        args.PropertyName == nameof(LoginViewModel.Username))
                    {
                        CommandManager.InvalidateRequerySuggested();
                    }
                };
            }
        }
    }
}
