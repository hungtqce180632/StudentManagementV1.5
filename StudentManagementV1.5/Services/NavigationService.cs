using System;
using System.Windows.Controls;

namespace StudentManagementV1._5.Services
{
    // Enum AppViews
    // + Tại sao cần sử dụng: Định nghĩa các màn hình trong ứng dụng để điều hướng
    // + Được sử dụng bởi NavigationService để xác định màn hình cần chuyển đến
    // + Chức năng chính: Cung cấp danh sách cố định các màn hình trong ứng dụng
    public enum AppViews
    {
        // 1. Các màn hình cơ bản
        // 2. Bao gồm đăng nhập và đặt lại mật khẩu
        // 3. Được hiển thị trước khi xác thực
        // Basic views
        Login,
        PasswordReset,
        
        // 1. Các màn hình Dashboard
        // 2. Màn hình chính cho từng loại người dùng
        // 3. Hiển thị sau khi xác thực
        // Dashboard views
        AdminDashboard,
        TeacherDashboard,
        StudentDashboard,
        
        // 1. Các màn hình quản lý cho Admin
        // 2. Bao gồm các chức năng quản lý hệ thống
        // 3. Chỉ Admin mới có quyền truy cập
        // Admin management views
        UserManagement,
        SubjectManagement,
        ExamManagement,
        ScoreManagement,
        ClassManagement,
        ScheduleManagement,
        NotificationManagement,
        
        // 1. Các màn hình dành cho Giáo viên
        // 2. Bao gồm các chức năng giảng dạy
        // 3. Chỉ Giáo viên mới có quyền truy cập
        // Teacher views
        AssignmentManagement,
        
        // 1. Các màn hình dành cho Học sinh
        // 2. Bao gồm các chức năng học tập
        // 3. Chỉ Học sinh mới có quyền truy cập
        // Student views
        SubmissionManagement,
        ViewAssignments
    }

    // Lớp NavigationService
    // + Tại sao cần sử dụng: Quản lý việc điều hướng giữa các màn hình trong ứng dụng
    // + Lớp này được gọi từ các ViewModel khi cần chuyển đổi màn hình
    // + Chức năng chính: Điều hướng qua lại giữa các màn hình và quản lý lịch sử điều hướng
    public class NavigationService
    {
        // 1. Frame chứa nội dung hiển thị
        // 2. Được sử dụng để nạp và hiển thị các màn hình
        // 3. Thường là frame chính của ứng dụng
        private readonly Frame _navigationFrame;
        
        // 1. Hàm để tạo UserControl từ loại màn hình
        // 2. Được truyền vào từ MainWindow
        // 3. Giúp chuyển đổi từ enum AppViews sang UserControl tương ứng
        private readonly Func<AppViews, UserControl> _pageResolver;

        // 1. Constructor của NavigationService
        // 2. Khởi tạo service với frame và hàm resolver
        // 3. Được gọi khi ứng dụng khởi động
        public NavigationService(Frame navigationFrame, Func<AppViews, UserControl> pageResolver)
        {
            _navigationFrame = navigationFrame;
            _pageResolver = pageResolver;
        }

        // 1. Phương thức điều hướng đến màn hình mới
        // 2. Nhận enum AppViews và chuyển thành UserControl
        // 3. Nạp UserControl vào frame để hiển thị
        public void NavigateTo(AppViews view)
        {
            var page = _pageResolver(view);
            _navigationFrame.Navigate(page);
        }

        // 1. Phương thức quay lại màn hình trước đó
        // 2. Kiểm tra xem có thể quay lại không
        // 3. Nếu có thể, thực hiện quay lại
        public void GoBack()
        {
            if (_navigationFrame.CanGoBack)
            {
                _navigationFrame.GoBack();
            }
        }
    }
}