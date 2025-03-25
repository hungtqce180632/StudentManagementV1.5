using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp AdminDashboardView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng chính cho quản trị viên sau khi đăng nhập
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng AdminDashboardViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này sau khi admin đăng nhập
     * 
     * Chức năng chính:
     * - Hiển thị tổng quan về hệ thống (số lượng học sinh, giáo viên, lớp học, môn học)
     * - Cung cấp các nút điều hướng đến các chức năng quản lý khác
     * - Hiển thị danh sách người dùng gần đây và các thao tác nhanh
     */
    public partial class AdminDashboardView : UserControl
    {
        public AdminDashboardView()
        {
            InitializeComponent();
        }
    }
}
