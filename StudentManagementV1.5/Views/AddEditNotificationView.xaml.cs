using StudentManagementV1._5.ViewModels;
using System.Windows;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp AddEditNotificationView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để thêm mới và chỉnh sửa thông báo
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng AddEditNotificationViewModel làm DataContext
     * - Đến lớp này: Được mở từ NotificationManagementViewModel khi thêm/sửa thông báo
     * 
     * Chức năng chính:
     * - Hiển thị form điền thông tin thông báo
     * - Cho phép chọn đối tượng nhận và thiết lập thời gian hết hạn
     * - Trả về kết quả cho view gọi nó
     */
    public partial class AddEditNotificationView : Window
    {
        // 1. Constructor của AddEditNotificationView
        // 2. Khởi tạo giao diện và thiết lập sự kiện
        // 3. Gán DataContext là AddEditNotificationViewModel
        public AddEditNotificationView()
        {
            InitializeComponent();
        }

        // 1. Constructor chấp nhận ViewModel làm tham số
        // 2. Gán ViewModel là DataContext
        // 3. Đăng ký xử lý sự kiện đóng dialog
        public AddEditNotificationView(AddEditNotificationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // Close the dialog when the ViewModel signals
            viewModel.RequestClose += (result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}
