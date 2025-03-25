using System.Windows;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp AddEditClassView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để thêm hoặc chỉnh sửa lớp học
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng AddEditClassViewModel làm DataContext
     * - Đến lớp này: ClassManagementViewModel mở hộp thoại này khi cần thêm/sửa lớp học
     * 
     * Chức năng chính:
     * - Hiển thị form để người dùng nhập thông tin lớp học
     * - Cho phép chọn giáo viên chủ nhiệm từ danh sách
     * - Xử lý sự kiện UI cơ bản
     */
    public partial class AddEditClassView : Window
    {
        public AddEditClassView()
        {
            InitializeComponent();
        }
    }
}
