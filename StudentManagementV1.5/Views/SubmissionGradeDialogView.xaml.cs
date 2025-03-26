using System.Windows;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp SubmissionGradeDialogView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để chấm điểm bài nộp của học sinh
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng SubmissionGradeViewModel làm DataContext
     * - Đến lớp này: Được mở từ SubmissionManagementViewModel khi chấm điểm bài nộp
     * 
     * Chức năng chính:
     * - Hiển thị form với thông tin của bài nộp và các điều khiển để chấm điểm
     * - Cho phép giáo viên nhập điểm và phản hồi
     */
    public partial class SubmissionGradeDialogView : Window
    {
        public SubmissionGradeDialogView()
        {
            InitializeComponent();
        }
    }
}
