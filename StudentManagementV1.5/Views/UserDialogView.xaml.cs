using StudentManagementV1._5.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp UserDialogView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để thêm mới và chỉnh sửa thông tin người dùng
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng UserDialogViewModel làm DataContext
     * - Đến lớp này: Được mở từ UserManagementViewModel khi thêm/sửa người dùng
     * 
     * Chức năng chính:
     * - Hiển thị form điền thông tin người dùng
     * - Xử lý việc truy cập vào PasswordBox (không thể binding trực tiếp vì lý do bảo mật)
     * - Trả về kết quả cho view gọi nó
     */
    public partial class UserDialogView : Window
    {
        // 1. Constructor của UserDialogView
        // 2. Khởi tạo giao diện và thiết lập sự kiện
        // 3. Gán DataContext là UserDialogViewModel
        public UserDialogView(UserDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // Pass the PasswordBox references to the ViewModel through a method
            viewModel.SetPasswordBoxes(PasswordBox, ConfirmPasswordBox);
            
            // Close the dialog when the ViewModel signals it
            viewModel.RequestClose += (result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}
