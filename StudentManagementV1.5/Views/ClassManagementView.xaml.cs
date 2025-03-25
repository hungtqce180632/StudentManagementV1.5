using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp ClassManagementView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để quản lý lớp học trong hệ thống
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng ClassManagementViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này khi cần quản lý lớp học
     * 
     * Chức năng chính:
     * - Hiển thị danh sách lớp học với khả năng tìm kiếm và lọc
     * - Cung cấp giao diện để tạo mới, chỉnh sửa và xóa lớp học
     */
    public partial class ClassManagementView : UserControl
    {
        public ClassManagementView()
        {
            InitializeComponent();
        }
    }
}
