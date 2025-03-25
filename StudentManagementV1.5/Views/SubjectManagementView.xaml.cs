using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp SubjectManagementView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để quản lý các môn học trong hệ thống
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng SubjectManagementViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này khi cần quản lý môn học
     * 
     * Chức năng chính:
     * - Hiển thị danh sách môn học với khả năng tìm kiếm và lọc
     * - Cung cấp giao diện để thêm, chỉnh sửa và xóa môn học
     */
    public partial class SubjectManagementView : UserControl
    {
        public SubjectManagementView()
        {
            InitializeComponent();
        }
    }
}
