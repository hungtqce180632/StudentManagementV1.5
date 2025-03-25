using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudentManagementV1._5.Views
{
    /*
     * Lớp ExamDialogView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng để thêm hoặc chỉnh sửa kỳ thi
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng ExamDialogViewModel làm DataContext
     * - Đến lớp này: ExamManagementViewModel mở hộp thoại này khi cần thêm/sửa kỳ thi
     * 
     * Chức năng chính:
     * - Hiển thị form để người dùng nhập thông tin kỳ thi
     * - Cho phép chọn môn học và lớp học từ danh sách
     * - Xử lý sự kiện UI cơ bản
     */
    public partial class ExamDialogView : Window
    {
        public ExamDialogView()
        {
            InitializeComponent();
        }
    }
}
