using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp TeacherDashboardView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng chính cho giáo viên sau khi đăng nhập
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng TeacherDashboardViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này sau khi giáo viên đăng nhập
     * 
     * Chức năng chính:
     * - Hiển thị tổng quan về các lớp học và bài tập mà giáo viên phụ trách
     * - Cung cấp điều hướng đến các chức năng dành cho giáo viên
     * - Hiển thị lịch dạy học sắp tới và các thao tác nhanh
     */
    public partial class TeacherDashboardView : UserControl
    {
        public TeacherDashboardView()
        {
            InitializeComponent();
        }
    }
}
