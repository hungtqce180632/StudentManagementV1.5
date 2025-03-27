using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Services;
using System;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp StudentDashboardViewModel
    // + Tại sao cần sử dụng: Cung cấp dữ liệu và xử lý logic cho màn hình Dashboard của Học sinh
    // + Lớp này được gọi từ NavigationService khi học sinh đăng nhập thành công
    // + Chức năng chính: Hiển thị tổng quan và cung cấp điều hướng đến các chức năng dành cho học sinh
    public class StudentDashboardViewModel : ViewModelBase
    {
        // 1. Dịch vụ xác thực người dùng
        // 2. Cung cấp thông tin về người dùng hiện tại
        // 3. Được truyền vào từ constructor
        private readonly AuthenticationService _authService;
        
        // 1. Dịch vụ điều hướng
        // 2. Dùng để chuyển đổi giữa các màn hình
        // 3. Được truyền vào từ constructor
        private readonly Services.NavigationService _navigationService;

        // 1. Thông điệp chào mừng hiển thị trên dashboard
        // 2. Binding đến TextBlock trong UI
        // 3. Được tạo dựa trên thông tin người dùng hiện tại
        private string _welcomeMessage = string.Empty;

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        // 1. Lệnh đăng xuất
        // 2. Binding đến nút "Đăng xuất" trong UI
        // 3. Khi được gọi, đăng xuất và chuyển về màn hình đăng nhập
        public ICommand LogoutCommand { get; }
        
        // 1. Lệnh điều hướng đến màn hình xem bài tập
        // 2. Binding đến nút "Xem bài tập" trong UI
        // 3. Khi được gọi, chuyển đến màn hình xem bài tập
        public ICommand NavigateToViewAssignmentsCommand { get; }
        
        // 1. Lệnh điều hướng đến màn hình quản lý bài nộp
        // 2. Binding đến nút "Quản lý bài nộp" trong UI
        // 3. Khi được gọi, chuyển đến màn hình quản lý bài nộp
        public ICommand NavigateToSubmissionManagementCommand { get; }

        // 1. Lệnh điều hướng đến màn hình khóa học của tôi
        // 2. Binding đến nút "Khóa học của tôi" trong UI
        // 3. Khi được gọi, chuyển đến màn hình hiển thị các khóa học học sinh đang học
        public ICommand NavigateToMyCoursesCommand { get; }

        // 1. Constructor của lớp
        // 2. Khởi tạo các tham số và thiết lập lệnh
        // 3. Tạo thông điệp chào mừng dựa trên thông tin người dùng
        public StudentDashboardViewModel(AuthenticationService authService, Services.NavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;

            WelcomeMessage = $"Welcome, {_authService.CurrentUser?.Username ?? "Student"}!";

            LogoutCommand = new RelayCommand(param => Logout());
            NavigateToViewAssignmentsCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.ViewAssignments));
            NavigateToSubmissionManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.SubmissionManagement));
            
            // Add new command for My Courses navigation
            NavigateToMyCoursesCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.MyCourses));
        }

        // 1. Phương thức đăng xuất
        // 2. Gọi dịch vụ xác thực để đăng xuất
        // 3. Chuyển về màn hình đăng nhập
        private void Logout()
        {
            _authService.Logout();
            _navigationService.NavigateTo(AppViews.Login);
        }
    }
}
