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
     * Lớp MySubjectsView
     * 
     * Tại sao sử dụng:
     * - Cung cấp giao diện người dùng cho màn hình hiển thị môn học của giáo viên
     * - Đóng vai trò là View trong mô hình MVVM
     * 
     * Quan hệ với các lớp khác:
     * - Từ lớp này: Sử dụng MySubjectsViewModel làm DataContext
     * - Đến lớp này: NavigationService điều hướng đến view này từ TeacherDashboard
     * 
     * Chức năng chính:
     * - Hiển thị danh sách các môn học mà giáo viên đang giảng dạy
     * - Cho phép xem danh sách học sinh và lịch học của môn học
     */
    public partial class MySubjectsView : UserControl
    {
        public MySubjectsView()
        {
            InitializeComponent();
        }
    }
}
