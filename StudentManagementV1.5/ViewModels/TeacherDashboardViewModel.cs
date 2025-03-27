using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp TeacherDashboardViewModel
    // + Tại sao cần sử dụng: Cung cấp dữ liệu và xử lý logic cho màn hình Dashboard của Giáo viên
    // + Lớp này được gọi từ NavigationService khi giáo viên đăng nhập thành công
    // + Chức năng chính: Hiển thị tổng quan và cung cấp điều hướng đến các chức năng dành cho giáo viên
    public class TeacherDashboardViewModel : ViewModelBase
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

        // 1. Thông điệp chào mừng hiển thị trên dashboard
        // 2. Binding đến TextBlock trong UI
        // 3. Được tạo dựa trên thông tin người dùng hiện tại
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        // 1. Lệnh đăng xuất
        // 2. Binding đến nút "Đăng xuất" trong UI
        // 3. Khi được gọi, đăng xuất và chuyển về màn hình đăng nhập
        public ICommand LogoutCommand { get; }
        
        // 1. Lệnh điều hướng đến màn hình quản lý bài tập
        // 2. Binding đến nút "Quản lý bài tập" trong UI
        // 3. Khi được gọi, chuyển đến màn hình quản lý bài tập
        public ICommand NavigateToAssignmentManagementCommand { get; }
        
        // 1. Lệnh điều hướng đến màn hình môn học của tôi
        // 2. Binding đến nút "Môn học của tôi" trong UI
        // 3. Khi được gọi, chuyển đến màn hình hiển thị các môn học giáo viên đang dạy
        public ICommand NavigateToMySubjectsCommand { get; }

        // Statistics for the dashboard
        private int _classCount;
        private int _studentCount;
        private int _assignmentCount;
        private int _pendingSubmissionsCount;
        private bool _isLoading;
        private string _teacherFullName = string.Empty;
        
        // Database service for querying statistics
        private readonly DatabaseService _databaseService;

        // Properties for the dashboard statistics
        public int ClassCount
        {
            get => _classCount;
            set => SetProperty(ref _classCount, value);
        }

        public int StudentCount
        {
            get => _studentCount;
            set => SetProperty(ref _studentCount, value);
        }

        public int AssignmentCount
        {
            get => _assignmentCount;
            set => SetProperty(ref _assignmentCount, value);
        }

        public int PendingSubmissionsCount
        {
            get => _pendingSubmissionsCount;
            set => SetProperty(ref _pendingSubmissionsCount, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string TeacherFullName
        {
            get => _teacherFullName;
            set => SetProperty(ref _teacherFullName, value);
        }

        // New navigation commands
        public ICommand NavigateToMyScheduleCommand { get; }
        public ICommand NavigateToNotificationsCommand { get; }
        public ICommand CreateNewAssignmentCommand { get; }
        public ICommand GradeSubmissionsCommand { get; }
        public ICommand ViewClassScheduleCommand { get; }
        public ICommand SendNotificationCommand { get; }

        // Make sure this command is defined and initialized correctly
        public ICommand NavigateCommand { get; }

        // 1. Constructor của lớp
        // 2. Khởi tạo các tham số và thiết lập lệnh
        // 3. Tạo thông điệp chào mừng dựa trên thông tin người dùng
        public TeacherDashboardViewModel(AuthenticationService authService, Services.NavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            _databaseService = new DatabaseService(); // Create a database service instance

            // Set the welcome message
            WelcomeMessage = $"Welcome, {_authService.CurrentUser?.Username ?? "Teacher"}!";

            // Initialize navigation commands
            LogoutCommand = new RelayCommand(param => Logout());
            NavigateToAssignmentManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.AssignmentManagement));
            NavigateToMySubjectsCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.MySubjects));
            
            // Initialize new navigation commands
            NavigateToMyScheduleCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.MySchedule));
            NavigateToNotificationsCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.NotificationManagement));
            
            // Quick action commands
            CreateNewAssignmentCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.AssignmentManagement));
            GradeSubmissionsCommand = new RelayCommand(param => MessageBox.Show("Grading interface will be implemented in a future update.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information));
            ViewClassScheduleCommand = new RelayCommand(param => MessageBox.Show("Class schedule view will be implemented in a future update.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information));
            SendNotificationCommand = new RelayCommand(param => MessageBox.Show("Notification sending will be implemented in a future update.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information));

            // Make sure this line exists to initialize the NavigateCommand
            NavigateCommand = new RelayCommand(param => Navigate(param as string));

            // Load dashboard data
            LoadDashboardDataAsync();
        }

        // Load statistics for the teacher dashboard
        private async void LoadDashboardDataAsync()
        {
            try
            {
                IsLoading = true;

                // Get the current teacher ID
                int teacherId = _authService.CurrentUser?.UserID ?? 0;
                if (teacherId == 0)
                {
                    MessageBox.Show("Teacher information not found. Please log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Load all dashboard data concurrently
                await Task.WhenAll(
                    LoadTeacherInfoAsync(teacherId),
                    LoadClassCountAsync(teacherId),
                    LoadStudentCountAsync(teacherId),
                    LoadAssignmentCountAsync(teacherId),
                    LoadPendingSubmissionsCountAsync(teacherId)
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Load teacher's personal information
        private async Task LoadTeacherInfoAsync(int teacherId)
        {
            string query = @"
                SELECT t.FirstName, t.LastName 
                FROM Teachers t
                WHERE t.UserID = @TeacherID";

            var parameters = new Dictionary<string, object>
            {
                { "@TeacherID", teacherId }
            };

            var result = await _databaseService.ExecuteQueryAsync(query, parameters);
            if (result.Rows.Count > 0)
            {
                string firstName = result.Rows[0]["FirstName"].ToString() ?? string.Empty;
                string lastName = result.Rows[0]["LastName"].ToString() ?? string.Empty;
                TeacherFullName = $"{firstName} {lastName}";
                
                // Update welcome message with full name
                WelcomeMessage = $"Welcome, {TeacherFullName}!";
            }
        }

        // Load count of classes taught by this teacher
        private async Task LoadClassCountAsync(int teacherId)
        {
            string query = @"
                SELECT COUNT(DISTINCT ClassID) AS ClassCount 
                FROM TeacherSubjects 
                WHERE TeacherID = @TeacherID";

            var parameters = new Dictionary<string, object>
            {
                { "@TeacherID", teacherId }
            };

            var result = await _databaseService.ExecuteScalarAsync(query, parameters);
            ClassCount = result != null ? Convert.ToInt32(result) : 0;
        }

        // Load count of students in classes taught by this teacher
        private async Task LoadStudentCountAsync(int teacherId)
        {
            string query = @"
                SELECT COUNT(DISTINCT s.StudentID) AS StudentCount 
                FROM Students s
                JOIN Classes c ON s.ClassID = c.ClassID
                JOIN TeacherSubjects ts ON c.ClassID = ts.ClassID
                WHERE ts.TeacherID = @TeacherID";

            var parameters = new Dictionary<string, object>
            {
                { "@TeacherID", teacherId }
            };

            var result = await _databaseService.ExecuteScalarAsync(query, parameters);
            StudentCount = result != null ? Convert.ToInt32(result) : 0;
        }

        // Load count of assignments created by this teacher
        private async Task LoadAssignmentCountAsync(int teacherId)
        {
            string query = @"
                SELECT COUNT(*) AS AssignmentCount 
                FROM Assignments 
                WHERE TeacherID = @TeacherID";

            var parameters = new Dictionary<string, object>
            {
                { "@TeacherID", teacherId }
            };

            var result = await _databaseService.ExecuteScalarAsync(query, parameters);
            AssignmentCount = result != null ? Convert.ToInt32(result) : 0;
        }

        // Load count of pending submissions to be reviewed
        private async Task LoadPendingSubmissionsCountAsync(int teacherId)
        {
            string query = @"
                SELECT COUNT(*) AS PendingCount 
                FROM Submissions s
                JOIN Assignments a ON s.AssignmentID = a.AssignmentID
                WHERE a.TeacherID = @TeacherID AND s.Status = 'Submitted'";

            var parameters = new Dictionary<string, object>
            {
                { "@TeacherID", teacherId }
            };

            var result = await _databaseService.ExecuteScalarAsync(query, parameters);
            PendingSubmissionsCount = result != null ? Convert.ToInt32(result) : 0;
        }

        // 1. Phương thức đăng xuất
        // 2. Gọi dịch vụ xác thực để đăng xuất
        // 3. Chuyển về màn hình đăng nhập
        private void Logout()
        {
            _authService.Logout();
            _navigationService.NavigateTo(AppViews.Login);
        }

        // Make sure this method exists to handle navigation
        private void Navigate(string viewName)
        {
            if (string.IsNullOrEmpty(viewName)) return;
    
            if (Enum.TryParse(viewName, out AppViews view))
            {
                _navigationService.NavigateTo(view);
            }
        }
    }
}
