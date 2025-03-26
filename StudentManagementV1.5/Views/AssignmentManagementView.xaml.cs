using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp AssignmentManagementView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng cho màn hình quản lý bài tập của giáo viên
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng AssignmentManagementViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này từ TeacherDashboard
     * 
     * Chức năng chính:
     * - Hiển thị danh sách bài tập đã giao
     * - Cho phép tạo bài tập mới, chỉnh sửa và xóa bài tập
     * - Quản lý bài nộp của học sinh
     */
    public partial class AssignmentManagementView : UserControl
    {
        public AssignmentManagementView()
        {
            InitializeComponent();
        }
    }
}
