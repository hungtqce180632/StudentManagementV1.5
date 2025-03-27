using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp MyCoursesView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để hiển thị danh sách khóa học của học sinh
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng MyCoursesViewModel làm DataContext
     * - Đến lớp này: Được điều hướng từ StudentDashboardView
     * 
     * Chức năng chính:
     * - Hiển thị danh sách khóa học học sinh đang theo học
     * - Cho phép tìm kiếm và lọc khóa học
     * - Xem thông tin chi tiết về khóa học
     */
    public partial class MyCoursesView : UserControl
    {
        public MyCoursesView()
        {
            InitializeComponent();
        }
    }
}
