using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp ExamManagementView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để quản lý kỳ thi trong hệ thống
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng ExamManagementViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này khi cần quản lý kỳ thi
     * 
     * Chức năng chính:
     * - Hiển thị danh sách kỳ thi với khả năng tìm kiếm và lọc
     * - Cung cấp giao diện để tạo mới, chỉnh sửa và xóa kỳ thi
     */
    public partial class ExamManagementView : UserControl
    {
        public ExamManagementView()
        {
            InitializeComponent();
        }
    }
}
