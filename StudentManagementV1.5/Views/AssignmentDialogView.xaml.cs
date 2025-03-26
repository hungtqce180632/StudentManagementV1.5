using System.Windows;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp AssignmentDialogView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để thêm mới hoặc chỉnh sửa bài tập
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng AssignmentDialogViewModel làm DataContext
     * - Đến lớp này: Được mở từ AssignmentManagementViewModel khi thêm/sửa bài tập
     * 
     * Chức năng chính:
     * - Hiển thị form với các trường thông tin của bài tập
     * - Cho phép nhập thông tin và lưu bài tập
     */
    public partial class AssignmentDialogView : Window
    {
        public AssignmentDialogView()
        {
            InitializeComponent();
        }
    }
}
