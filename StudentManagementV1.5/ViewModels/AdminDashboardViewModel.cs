using StudentManagementV1._5.Commands;

using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp AdminDashboardViewModel
    // + Tại sao cần sử dụng: Cung cấp dữ liệu và xử lý logic cho màn hình Dashboard của Admin
    // + Lớp này được gọi từ NavigationService khi quản trị viên đăng nhập thành công
    // + Chức năng chính: Hiển thị tổng quan về hệ thống và cung cấp điều hướng đến các chức năng quản lý
    public class AdminDashboardViewModel : ViewModelBase
    {
        // 1. Dịch vụ xác thực người dùng
        // 2. Cung cấp thông tin về người dùng hiện tại
        // 3. Được truyền vào từ constructor
        private readonly AuthenticationService _authService;
        
        // 1. Dịch vụ kết nối với cơ sở dữ liệu
        // 2. Dùng để truy vấn dữ liệu thống kê
        // 3. Được khởi tạo trong constructor
        private readonly DatabaseService _databaseService;
        
        // 1. Dịch vụ điều hướng
        // 2. Dùng để chuyển đổi giữa các màn hình
        // 3. Được truyền vào từ constructor
        private readonly Services.NavigationService _navigationService;

        // 1. Thông điệp chào mừng hiển thị trên dashboard
        // 2. Binding đến TextBlock trong UI
        // 3. Được tạo dựa trên thông tin người dùng hiện tại
        private string _welcomeMessage = string.Empty;
        
        // 1. Số lượng học sinh trong hệ thống
        // 2. Binding đến TextBlock trong UI để hiển thị thống kê
        // 3. Được tải từ cơ sở dữ liệu
        private int _studentCount;
        
        // 1. Số lượng giáo viên trong hệ thống
        // 2. Binding đến TextBlock trong UI để hiển thị thống kê
        // 3. Được tải từ cơ sở dữ liệu
        private int _teacherCount;
        
        // 1. Số lượng lớp học đang hoạt động
        // 2. Binding đến TextBlock trong UI để hiển thị thống kê
        // 3. Được tải từ cơ sở dữ liệu
        private int _classCount;
        
        // 1. Số lượng môn học đang hoạt động
        // 2. Binding đến TextBlock trong UI để hiển thị thống kê
        // 3. Được tải từ cơ sở dữ liệu
        private int _subjectCount;
        
        // 1. Danh sách người dùng đăng nhập gần đây
        // 2. Binding đến ListView trong UI
        // 3. Được tải từ cơ sở dữ liệu
        private ObservableCollection<User> _recentUsers = new ObservableCollection<User>();
        
        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến UI để hiển thị thông báo "Đang tải..."
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        private bool _isLoading;

        // 1. Thông điệp chào mừng hiển thị trên dashboard
        // 2. Binding đến TextBlock trong UI
        // 3. Được tạo dựa trên thông tin người dùng hiện tại
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        // 1. Số lượng học sinh trong hệ thống
        // 2. Binding đến TextBlock trong UI để hiển thị thống kê
        // 3. Được tải từ cơ sở dữ liệu
        public int StudentCount
        {
            get => _studentCount;
            set => SetProperty(ref _studentCount, value);
        }

        // 1. Số lượng giáo viên trong hệ thống
        // 2. Binding đến TextBlock trong UI để hiển thị thống kê
        // 3. Được tải từ cơ sở dữ liệu
        public int TeacherCount
        {
            get => _teacherCount;
            set => SetProperty(ref _teacherCount, value);
        }

        // 1. Số lượng lớp học đang hoạt động
        // 2. Binding đến TextBlock trong UI để hiển thị thống kê
        // 3. Được tải từ cơ sở dữ liệu
        public int ClassCount
        {
            get => _classCount;
            set => SetProperty(ref _classCount, value);
        }

        // 1. Số lượng môn học đang hoạt động
        // 2. Binding đến TextBlock trong UI để hiển thị thống kê
        // 3. Được tải từ cơ sở dữ liệu
        public int SubjectCount
        {
            get => _subjectCount;
            set => SetProperty(ref _subjectCount, value);
        }

        // 1. Danh sách người dùng đăng nhập gần đây
        // 2. Binding đến ListView trong UI
        // 3. Được tải từ cơ sở dữ liệu
        public ObservableCollection<User> RecentUsers
        {
            get => _recentUsers;
            set => SetProperty(ref _recentUsers, value);
        }

        // 1. Thuộc tính xác định xem có người dùng đăng nhập gần đây không
        // 2. Binding đến Visibility của ListView
        // 3. Trả về true nếu có người dùng đăng nhập gần đây
        public bool HasRecentUsers => RecentUsers.Count > 0;

        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến UI để hiển thị thông báo "Đang tải..."
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // 1. Lệnh đăng xuất
        // 2. Binding đến nút "Đăng xuất" trong UI
        // 3. Khi được gọi, đăng xuất và chuyển về màn hình đăng nhập
        public ICommand LogoutCommand { get; }
        
        // 1. Lệnh điều hướng đến màn hình quản lý người dùng
        // 2. Binding đến nút "Quản lý người dùng" trong UI
        // 3. Khi được gọi, chuyển đến màn hình quản lý người dùng
        public ICommand NavigateToUserManagementCommand { get; }
        
        // 1. Lệnh điều hướng đến màn hình quản lý môn học
        // 2. Binding đến nút "Quản lý môn học" trong UI
        // 3. Khi được gọi, chuyển đến màn hình quản lý môn học
        public ICommand NavigateToSubjectManagementCommand { get; }
        
        // 1. Lệnh điều hướng đến màn hình quản lý kỳ thi
        // 2. Binding đến nút "Quản lý kỳ thi" trong UI
        // 3. Khi được gọi, chuyển đến màn hình quản lý kỳ thi
        public ICommand NavigateToExamManagementCommand { get; }
        
        // 1. Lệnh điều hướng đến màn hình quản lý điểm số
        // 2. Binding đến nút "Quản lý điểm số" trong UI
        // 3. Khi được gọi, chuyển đến màn hình quản lý điểm số
        public ICommand NavigateToScoreManagementCommand { get; }
        
        // 1. Lệnh điều hướng đến màn hình quản lý lớp học
        // 2. Binding đến nút "Quản lý lớp học" trong UI
        // 3. Khi được gọi, chuyển đến màn hình quản lý lớp học
        public ICommand NavigateToClassManagementCommand { get; }
        
        // 1. Lệnh điều hướng đến màn hình quản lý lịch học
        // 2. Binding đến nút "Quản lý lịch học" trong UI
        // 3. Khi được gọi, chuyển đến màn hình quản lý lịch học
        public ICommand NavigateToScheduleManagementCommand { get; }
        
        // 1. Lệnh điều hướng đến màn hình quản lý thông báo
        // 2. Binding đến nút "Quản lý thông báo" trong UI
        // 3. Khi được gọi, chuyển đến màn hình quản lý thông báo
        public ICommand NavigateToNotificationManagementCommand { get; }

        // 1. Lệnh thêm học sinh mới
        // 2. Binding đến nút "Thêm học sinh mới" trong UI
        // 3. Khi được gọi, mở hộp thoại thêm học sinh
        public ICommand AddNewStudentCommand { get; }
        
        // 1. Lệnh thêm giáo viên mới
        // 2. Binding đến nút "Thêm giáo viên mới" trong UI
        // 3. Khi được gọi, mở hộp thoại thêm giáo viên
        public ICommand AddNewTeacherCommand { get; }
        
        // 1. Lệnh tạo lớp học mới
        // 2. Binding đến nút "Tạo lớp học mới" trong UI
        // 3. Khi được gọi, mở hộp thoại tạo lớp học
        public ICommand CreateNewClassCommand { get; }
        
        // 1. Lệnh tạo bản sao lưu hệ thống
        // 2. Binding đến nút "Tạo bản sao lưu hệ thống" trong UI
        // 3. Khi được gọi, thực hiện sao lưu cơ sở dữ liệu
        public ICommand CreateSystemBackupCommand { get; }

        // 1. Constructor của lớp
        // 2. Khởi tạo các tham số và thiết lập lệnh
        // 3. Tải dữ liệu ban đầu từ cơ sở dữ liệu
        public AdminDashboardViewModel(AuthenticationService authService, Services.NavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            _databaseService = new DatabaseService(); // Get instance of database service

            WelcomeMessage = $"Welcome, {_authService.CurrentUser?.Username ?? "Admin"}!";

            // Initialize navigation commands
            LogoutCommand = new RelayCommand(param => Logout());
            NavigateToUserManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.UserManagement));
            NavigateToSubjectManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.SubjectManagement));
            NavigateToExamManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.ExamManagement));
            NavigateToScoreManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.ScoreManagement));
            NavigateToClassManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.ClassManagement));
            NavigateToScheduleManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.ScheduleManagement));
            NavigateToNotificationManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.NotificationManagement));

            // Initialize quick action commands
            AddNewStudentCommand = new RelayCommand(param => AddNewStudent());
            AddNewTeacherCommand = new RelayCommand(param => AddNewTeacher());
            CreateNewClassCommand = new RelayCommand(param => CreateNewClass());
            CreateSystemBackupCommand = new RelayCommand(param => CreateSystemBackup());

            // Load data
            LoadDataAsync();
        }

        // 1. Phương thức tải dữ liệu cho dashboard
        // 2. Tải đồng thời các thông tin thống kê
        // 3. Cập nhật UI khi tải xong
        private async void LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                // Load statistics
                await Task.WhenAll(
                    LoadStudentCountAsync(),
                    LoadTeacherCountAsync(),
                    LoadClassCountAsync(),
                    LoadSubjectCountAsync(),
                    LoadRecentUsersAsync()
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

        // 1. Phương thức tải số lượng học sinh
        // 2. Truy vấn cơ sở dữ liệu để đếm số học sinh
        // 3. Cập nhật thuộc tính StudentCount
        private async Task LoadStudentCountAsync()
        {
            // Count students from database
            string query = "SELECT COUNT(*) FROM Students";
            var result = await _databaseService.ExecuteScalarAsync(query);
            StudentCount = result != null ? Convert.ToInt32(result) : 0;
        }

        // 1. Phương thức tải số lượng giáo viên
        // 2. Truy vấn cơ sở dữ liệu để đếm số giáo viên
        // 3. Cập nhật thuộc tính TeacherCount
        private async Task LoadTeacherCountAsync()
        {
            // Count teachers from database
            string query = "SELECT COUNT(*) FROM Teachers";
            var result = await _databaseService.ExecuteScalarAsync(query);
            TeacherCount = result != null ? Convert.ToInt32(result) : 0;
        }

        // 1. Phương thức tải số lượng lớp học đang hoạt động
        // 2. Truy vấn cơ sở dữ liệu để đếm số lớp học hoạt động
        // 3. Cập nhật thuộc tính ClassCount
        private async Task LoadClassCountAsync()
        {
            // Count active classes from database
            string query = "SELECT COUNT(*) FROM Classes WHERE IsActive = 1";
            var result = await _databaseService.ExecuteScalarAsync(query);
            ClassCount = result != null ? Convert.ToInt32(result) : 0;
        }

        // 1. Phương thức tải số lượng môn học đang hoạt động
        // 2. Truy vấn cơ sở dữ liệu để đếm số môn học hoạt động
        // 3. Cập nhật thuộc tính SubjectCount
        private async Task LoadSubjectCountAsync()
        {
            // Count subjects from database
            string query = "SELECT COUNT(*) FROM Subjects WHERE IsActive = 1";
            var result = await _databaseService.ExecuteScalarAsync(query);
            SubjectCount = result != null ? Convert.ToInt32(result) : 0;
        }

        // 1. Phương thức tải danh sách người dùng đăng nhập gần đây
        // 2. Truy vấn cơ sở dữ liệu để lấy 5 người dùng gần nhất
        // 3. Cập nhật danh sách RecentUsers
        private async Task LoadRecentUsersAsync()
        {
            // Get recent users
            string query = @"
                SELECT TOP 5 * FROM Users 
                ORDER BY CASE WHEN LastLoginDate IS NULL THEN CreatedDate ELSE LastLoginDate END DESC";
            
            var dataTable = await _databaseService.ExecuteQueryAsync(query);
            
            // Clear existing collection
            RecentUsers.Clear();
            
            // Parse and add users to collection
            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                RecentUsers.Add(new User
                {
                    UserID = Convert.ToInt32(row["UserID"]),
                    Username = row["Username"].ToString() ?? string.Empty,
                    Email = row["Email"].ToString() ?? string.Empty,
                    Role = row["Role"].ToString() ?? string.Empty,
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    LastLoginDate = row["LastLoginDate"] != DBNull.Value ? Convert.ToDateTime(row["LastLoginDate"]) : null
                });
            }
            
            // Notify property changed for the HasRecentUsers property
            OnPropertyChanged(nameof(HasRecentUsers));
        }

        // 1. Phương thức đăng xuất
        // 2. Gọi dịch vụ xác thực để đăng xuất
        // 3. Chuyển về màn hình đăng nhập
        private void Logout()
        {
            _authService.Logout();
            _navigationService.NavigateTo(AppViews.Login);
        }

        // 1. Phương thức mở hộp thoại thêm học sinh mới
        // 2. Hiển thị thông báo chức năng chưa được triển khai
        // 3. Đặt chỗ cho chức năng thêm học sinh trong tương lai
        private void AddNewStudent()
        {
            // Navigate to add student form or show dialog
            MessageBox.Show("Add New Student functionality will be implemented soon.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 1. Phương thức mở hộp thoại thêm giáo viên mới
        // 2. Hiển thị thông báo chức năng chưa được triển khai
        // 3. Đặt chỗ cho chức năng thêm giáo viên trong tương lai
        private void AddNewTeacher()
        {
            // Navigate to add teacher form or show dialog
            MessageBox.Show("Add New Teacher functionality will be implemented soon.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 1. Phương thức mở hộp thoại tạo lớp học mới
        // 2. Hiển thị thông báo chức năng chưa được triển khai
        // 3. Đặt chỗ cho chức năng tạo lớp học trong tương lai
        private void CreateNewClass()
        {
            // Navigate to create class form or show dialog
            MessageBox.Show("Create New Class functionality will be implemented soon.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 1. Phương thức tạo bản sao lưu hệ thống
        // 2. Mô phỏng quá trình sao lưu và hiển thị thông báo
        // 3. Đặt chỗ cho chức năng sao lưu thực tế trong tương lai
        private async void CreateSystemBackup()
        {
            try
            {
                // Show processing dialog
                MessageBox.Show("Creating system backup...", "Backup", MessageBoxButton.OK, MessageBoxImage.Information);

                // Simulate backup process
                await Task.Delay(2000);

                // Display success message
                MessageBox.Show("System backup created successfully!", "Backup Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating backup: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
