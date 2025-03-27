using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp MyScheduleView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để hiển thị lịch dạy của giáo viên
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng MyScheduleViewModel làm DataContext
     * - Đến lớp này: Được điều hướng từ TeacherDashboardView
     * 
     * Chức năng chính:
     * - Hiển thị giao diện lịch dạy
     * - Cho phép lọc theo ngày trong tuần
     */
    public partial class MyScheduleView : UserControl
    {
        public MyScheduleView()
        {
            InitializeComponent();
        }
    }
}
