using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp UserManagementView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để quản lý tài khoản trong hệ thống
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng UserManagementViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này khi cần quản lý người dùng
     * 
     * Chức năng chính:
     * - Hiển thị danh sách người dùng với khả năng tìm kiếm và lọc
     * - Cung cấp giao diện để thêm, chỉnh sửa và xóa người dùng
     */
    public partial class UserManagementView : UserControl
    {
        public UserManagementView()
        {
            InitializeComponent();
        }
    }
}
