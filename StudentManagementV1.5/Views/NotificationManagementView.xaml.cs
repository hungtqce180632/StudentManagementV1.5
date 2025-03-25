using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp NotificationManagementView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để quản lý thông báo trong hệ thống
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng NotificationManagementViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này khi cần quản lý thông báo
     * 
     * Chức năng chính:
     * - Hiển thị danh sách thông báo với khả năng tìm kiếm và lọc
     * - Cung cấp giao diện để tạo mới, xem chi tiết, sửa và xóa thông báo
     */
    public partial class NotificationManagementView : UserControl
    {
        public NotificationManagementView()
        {
            InitializeComponent();
        }
    }
}
