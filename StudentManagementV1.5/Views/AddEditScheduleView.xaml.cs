using StudentManagementV1._5.ViewModels;
using System.Windows;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp AddEditScheduleView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để thêm mới và chỉnh sửa lịch học
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng AddEditScheduleViewModel làm DataContext
     * - Đến lớp này: Được mở từ ScheduleManagementViewModel khi thêm/sửa lịch học
     * 
     * Chức năng chính:
     * - Hiển thị form điền thông tin lịch học
     * - Cho phép chọn lớp, môn học, giáo viên, thời gian và phòng học
     * - Trả về kết quả cho view gọi nó
     */
    public partial class AddEditScheduleView : Window
    {
        // 1. Constructor của AddEditScheduleView
        // 2. Khởi tạo giao diện và thiết lập sự kiện
        // 3. Gán DataContext là AddEditScheduleViewModel
        public AddEditScheduleView()
        {
            InitializeComponent();
        }

        // 1. Constructor chấp nhận ViewModel làm tham số
        // 2. Gán ViewModel là DataContext
        // 3. Đăng ký xử lý sự kiện đóng dialog
        public AddEditScheduleView(AddEditScheduleViewModel viewModel)
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
