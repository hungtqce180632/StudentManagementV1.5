using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp ScheduleManagementView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để quản lý lịch học trong hệ thống
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng ScheduleManagementViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này khi cần quản lý lịch học
     * 
     * Chức năng chính:
     * - Hiển thị danh sách lịch học với khả năng lọc theo lớp và ngày
     * - Cung cấp giao diện để thêm, chỉnh sửa và xóa lịch học
     */
    public partial class ScheduleManagementView : UserControl
    {
        public ScheduleManagementView()
        {
            InitializeComponent();
        }
    }
}
