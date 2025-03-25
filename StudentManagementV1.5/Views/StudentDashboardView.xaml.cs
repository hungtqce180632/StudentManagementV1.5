using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp StudentDashboardView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng chính cho học sinh sau khi đăng nhập
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng StudentDashboardViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này sau khi học sinh đăng nhập
     * 
     * Chức năng chính:
     * - Hiển thị tổng quan về thông tin học tập của học sinh
     * - Cung cấp điều hướng đến các chức năng dành cho học sinh
     * - Hiển thị danh sách bài tập sắp đến hạn và điểm số gần đây
     */
    public partial class StudentDashboardView : UserControl
    {
        public StudentDashboardView()
        {
            InitializeComponent();
        }
    }
}
