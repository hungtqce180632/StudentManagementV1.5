using System.Windows;
using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp PasswordResetView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để đặt lại mật khẩu khi người dùng quên mật khẩu
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng PasswordResetViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này từ màn hình đăng nhập
     * 
     * Chức năng chính:
     * - Cho phép người dùng yêu cầu mã đặt lại mật khẩu
     * - Xác thực mã và nhập mật khẩu mới
     * - Hiển thị thông báo kết quả quá trình đặt lại mật khẩu
     */
    public partial class PasswordResetView : UserControl
    {
        public PasswordResetView()
        {
            InitializeComponent();
        }
    }
}
