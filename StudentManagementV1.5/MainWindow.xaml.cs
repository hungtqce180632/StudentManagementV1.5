using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Services;
using StudentManagementV1._5.ViewModels;
using StudentManagementV1._5.Views;

namespace StudentManagementV1._5;

/*
 * Lớp MainWindow
 * 
 * Tại sao sử dụng:
 * - Là cửa sổ chính của ứng dụng, chịu trách nhiệm quản lý chế độ xem của toàn bộ ứng dụng
 * - Triển khai INotifyPropertyChanged để hỗ trợ binding giữa UI và logic
 * 
 * Quan hệ với các lớp khác:
 * - Khởi tạo và quản lý các dịch vụ chính: DatabaseService, AuthenticationService, EmailService, NavigationService
 * - Điều hướng giữa các màn hình khác nhau dựa trên trạng thái đăng nhập và vai trò người dùng
 * 
 * Chức năng chính:
 * - Điều khiển luồng chuyển đổi giữa màn hình đăng nhập và các màn hình chính của ứng dụng
 * - Khởi tạo các dịch vụ cần thiết và phân giải các view khi điều hướng
 */
public partial class MainWindow : Window, INotifyPropertyChanged
{
    // 1. Dịch vụ kết nối với cơ sở dữ liệu
    // 2. Được sử dụng bởi các dịch vụ khác để truy xuất và cập nhật dữ liệu
    // 3. Được khởi tạo trong constructor
    private readonly DatabaseService _databaseService;
    
    // 1. Dịch vụ xác thực người dùng
    // 2. Sử dụng để quản lý đăng nhập, đăng xuất và thông tin người dùng hiện tại
    // 3. Được khởi tạo trong constructor và truyền vào DatabaseService
    private readonly AuthenticationService _authService;
    
    // 1. Dịch vụ gửi email
    // 2. Sử dụng để gửi thông báo và email đặt lại mật khẩu
    // 3. Được khởi tạo trong constructor
    private readonly EmailService _emailService;
    
    // 1. Dịch vụ điều hướng
    // 2. Sử dụng để chuyển đổi giữa các màn hình khác nhau
    // 3. Được khởi tạo trong constructor với MainFrame và hàm ResolveView
    private readonly Services.NavigationService _navigationService;
    
    // 1. Trạng thái hiển thị của màn hình đăng nhập
    // 2. Binding đến Visibility của MainContent và MainFrame
    // 3. True khi hiển thị màn hình đăng nhập, False khi hiển thị nội dung chính
    private bool _isLoginVisible = true;
    
    // 1. ViewModel của màn hình đăng nhập
    // 2. Chứa logic xử lý đăng nhập và điều hướng
    // 3. Được khởi tạo trong constructor và sử dụng trong ShowLoginView
    private LoginViewModel _loginViewModel;

    // 1. Sự kiện thông báo khi thuộc tính thay đổi
    // 2. Đăng ký với binding trong XAML
    // 3. Cần thiết để hiển thị cập nhật trạng thái giao diện
    public event PropertyChangedEventHandler? PropertyChanged;

    // 1. Thuộc tính binding đến Visibility trong XAML
    // 2. Điều khiển hiển thị của màn hình đăng nhập và nội dung chính
    // 3. Thông báo thay đổi thông qua OnPropertyChanged
    public bool IsLoginVisible
    {
        get => _isLoginVisible;
        set
        {
            _isLoginVisible = value;
            OnPropertyChanged();
        }
    }

    // 1. Constructor chính của MainWindow
    // 2. Khởi tạo các dịch vụ, thiết lập điều hướng và hiển thị màn hình đăng nhập
    // 3. Đặt DataContext là chính MainWindow để hỗ trợ binding
    public MainWindow()
    {
        // Set DataContext to this instance for bindings
        DataContext = this;
        
        InitializeComponent();

        // Initialize services
        _databaseService = new DatabaseService();
        _authService = new AuthenticationService(_databaseService);
        _emailService = new EmailService();
        
        // Setup navigation with parameter support
        _navigationService = new Services.NavigationService(MainFrame, ResolveView, ResolveViewWithParameter);
        
        // Setup login view
        _loginViewModel = new LoginViewModel(_authService, _navigationService);
        _loginViewModel.LoginSuccessful += OnLoginSuccessful;
        ShowLoginView();
    }

    // 1. Phương thức hiển thị màn hình đăng nhập
    // 2. Tạo LoginView với LoginViewModel làm DataContext
    // 3. Gán LoginView làm nội dung của MainContent
    private void ShowLoginView()
    {
        // Create the login view and set its DataContext
        var loginView = new LoginView();
        loginView.DataContext = _loginViewModel;
        
        // Set the login view as the content of the main window
        MainContent.Content = loginView;
    }

    // 1. Xử lý sự kiện đăng nhập thành công
    // 2. Ẩn màn hình đăng nhập và hiển thị nội dung chính
    // 3. Điều hướng đến dashboard tương ứng với vai trò người dùng
    private void OnLoginSuccessful(object sender, EventArgs e)
    {
        // Hide login view and show main content
        IsLoginVisible = false;
        
        // Navigate to the appropriate dashboard based on user role
        switch (_authService.CurrentUser?.Role)
        {
            case "Admin":
                _navigationService.NavigateTo(AppViews.AdminDashboard);
                break;
            case "Teacher":
                _navigationService.NavigateTo(AppViews.TeacherDashboard);
                break;
            case "Student":
                _navigationService.NavigateTo(AppViews.StudentDashboard);
                break;
            default:
                MessageBox.Show("Unknown user role", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                break;
        }
    }

    // 1. Phương thức phân giải các view từ enum AppViews
    // 2. Tạo instance của view tương ứng với DataContext phù hợp
    // 3. Được gọi bởi NavigationService khi cần chuyển đổi view
    private UserControl ResolveView(AppViews view)
    {
        switch (view)
        {
            // Basic views
            case AppViews.Login:
                return new LoginView { DataContext = new LoginViewModel(_authService, _navigationService) };
            case AppViews.PasswordReset:
                return new PasswordResetView { DataContext = new PasswordResetViewModel(_authService, _emailService, _navigationService) };
            
            // Dashboard views
            case AppViews.AdminDashboard:
                return new AdminDashboardView { DataContext = new AdminDashboardViewModel(_authService, _navigationService) };
            case AppViews.TeacherDashboard:
                return new TeacherDashboardView { DataContext = new TeacherDashboardViewModel(_authService, _navigationService) };
            case AppViews.StudentDashboard:
                return new StudentDashboardView { DataContext = new StudentDashboardViewModel(_authService, _navigationService) };
            
            // Admin management views
            case AppViews.UserManagement:
                return new UserManagementView { DataContext = new UserManagementViewModel(_databaseService, _authService, _navigationService) };
            case AppViews.ScheduleManagement:
                return new ScheduleManagementView { DataContext = new ScheduleManagementViewModel(_databaseService, _navigationService) };
            case AppViews.ClassManagement:
                return new ClassManagementView { DataContext = new ClassManagementViewModel(_databaseService, _navigationService) };
            case AppViews.SubjectManagement:
                return new SubjectManagementView { DataContext = new SubjectManagementViewModel(_databaseService, _navigationService) };
            case AppViews.NotificationManagement:
                return new NotificationManagementView { DataContext = new NotificationManagementViewModel(_databaseService, _authService, _navigationService) };
            case AppViews.ExamManagement:
                return new ExamManagementView { DataContext = new ExamManagementViewModel(_databaseService, _navigationService) };
            case AppViews.AssignmentManagement:
                return new AssignmentManagementView { DataContext = new AssignmentManagementViewModel(_databaseService, _navigationService, _authService) };
            case AppViews.MySubjects:
                return new MySubjectsView { DataContext = new MySubjectsViewModel(_databaseService, _navigationService, _authService) };

            // Add the rest of the views as you implement them
            default:
                throw new NotImplementedException($"View {view} is not implemented");
        }
    }

    // 1. Phương thức phân giải các view từ enum AppViews có truyền tham số
    // 2. Tạo instance của view tương ứng với DataContext phù hợp và truyền tham số
    // 3. Được gọi bởi NavigationService khi cần chuyển đổi view có tham số
    private UserControl ResolveViewWithParameter(AppViews view, object parameter)
    {
        switch (view)
        {
            case AppViews.SubmissionManagement:
                return new SubmissionManagementView { DataContext = new SubmissionManagementViewModel(_databaseService, _navigationService, _authService) };
            
            // Add other views that need parameters here
            
            default:
                // Fall back to regular view resolution
                return ResolveView(view);
        }
    }

    // 1. Phương thức thông báo khi thuộc tính thay đổi
    // 2. Kích hoạt sự kiện PropertyChanged với tên thuộc tính
    // 3. Sử dụng CallerMemberName để tự động lấy tên thuộc tính
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}