using System.Windows;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp AddEditNotificationView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để thêm hoặc chỉnh sửa thông báo
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng AddEditNotificationViewModel làm DataContext
     * - Đến lớp này: NotificationManagementViewModel mở hộp thoại này khi cần thêm/sửa thông báo
     * 
     * Chức năng chính:
     * - Hiển thị form để người dùng nhập thông tin thông báo
     * - Cho phép chọn loại người nhận và người nhận cụ thể
     * - Xử lý sự kiện UI cơ bản
     */
    public partial class AddEditNotificationView : Window
    {
        public AddEditNotificationView()
        {
            InitializeComponent();
        }
    }
}
