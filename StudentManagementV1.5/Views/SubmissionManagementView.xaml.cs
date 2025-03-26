using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp SubmissionManagementView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng cho màn hình quản lý bài nộp
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng SubmissionManagementViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này từ AssignmentManagementView
     * 
     * Chức năng chính:
     * - Hiển thị danh sách bài nộp của học sinh cho một bài tập
     * - Cho phép xem chi tiết bài nộp và chấm điểm
     */
    public partial class SubmissionManagementView : UserControl
    {
        public SubmissionManagementView()
        {
            InitializeComponent();
        }
    }
}
