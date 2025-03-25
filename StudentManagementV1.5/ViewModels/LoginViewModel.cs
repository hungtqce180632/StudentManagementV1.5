using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp LoginViewModel
    // + Tại sao cần sử dụng: Quản lý quá trình đăng nhập của người dùng
    // + Lớp này được gọi từ màn hình đăng nhập và là điểm vào chính của ứng dụng
    // + Chức năng chính: Xác thực người dùng và điều hướng đến màn hình phù hợp sau đăng nhập
    public class LoginViewModel : ViewModelBase
    {
        // 1. Dịch vụ xác thực người dùng
        // 2. Được truyền vào từ constructor
        // 3. Cung cấp phương thức đăng nhập và kiểm tra thông tin người dùng
        private readonly AuthenticationService _authService;
        
        // 1. Dịch vụ điều hướng
        // 2. Được truyền vào từ constructor
        // 3. Dùng để chuyển đến màn hình phù hợp sau đăng nhập
        private readonly NavigationService _navigationService;

        // 1. Tên đăng nhập người dùng nhập vào
        // 2. Binding đến TextBox trong giao diện
        // 3. Được sử dụng để xác thực đăng nhập
        private string _username = string.Empty;
        
        // 1. Mật khẩu người dùng nhập vào
        // 2. Binding đến PasswordBox trong giao diện
        // 3. Được sử dụng để xác thực đăng nhập
        private string _password = string.Empty;
        
        // 1. Thông báo lỗi khi đăng nhập không thành công
        // 2. Binding đến TextBlock trong giao diện
        // 3. Hiển thị lỗi cho người dùng
        private string _errorMessage = string.Empty;
        
        // 1. Trạng thái đang tải
        // 2. Binding đến trạng thái hiển thị của ProgressBar
        // 3. Cập nhật khi bắt đầu và kết thúc quá trình đăng nhập
        private bool _isLoading;

        // 1. Sự kiện thông báo đăng nhập thành công
        // 2. Được đăng ký bởi MainWindow để xử lý sau đăng nhập
        // 3. Kích hoạt khi người dùng đăng nhập thành công
        public event EventHandler? LoginSuccessful;

        // 1. Tên đăng nhập người dùng nhập vào
        // 2. Binding đến TextBox trong giao diện
        // 3. Được sử dụng để xác thực đăng nhập
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        // 1. Mật khẩu người dùng nhập vào
        // 2. Binding đến PasswordBox trong giao diện
        // 3. Được sử dụng để xác thực đăng nhập
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        // 1. Thông báo lỗi khi đăng nhập không thành công
        // 2. Binding đến TextBlock trong giao diện
        // 3. Hiển thị lỗi cho người dùng
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // 1. Trạng thái đang tải
        // 2. Binding đến trạng thái hiển thị của ProgressBar
        // 3. Cập nhật khi bắt đầu và kết thúc quá trình đăng nhập
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // 1. Lệnh đăng nhập
        // 2. Binding đến nút "Login" trong giao diện
        // 3. Thực hiện xác thực người dùng khi được kích hoạt
        public ICommand LoginCommand { get; }
        
        // 1. Lệnh quên mật khẩu
        // 2. Binding đến liên kết "Forgot Password" trong giao diện
        // 3. Điều hướng đến màn hình đặt lại mật khẩu
        public ICommand ForgotPasswordCommand { get; }

        // 1. Constructor của lớp
        // 2. Nhận các dịch vụ cần thiết
        // 3. Khởi tạo các lệnh và thiết lập trạng thái ban đầu
        public LoginViewModel(AuthenticationService authService, NavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;

            LoginCommand = new RelayCommand(async param => await Login(), 
                param => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !IsLoading);
            
            ForgotPasswordCommand = new RelayCommand(param => NavigateToPasswordReset());
        }

        // 1. Phương thức đăng nhập
        // 2. Gọi AuthService để xác thực người dùng
        // 3. Kích hoạt sự kiện LoginSuccessful khi đăng nhập thành công
        private async Task Login()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                bool isAuthenticated = await _authService.LoginAsync(Username, Password);

                if (isAuthenticated)
                {
                    // Raise the LoginSuccessful event instead of navigating directly
                    LoginSuccessful?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // 1. Phương thức điều hướng đến màn hình đặt lại mật khẩu
        // 2. Sử dụng NavigationService để chuyển màn hình
        // 3. Được gọi khi người dùng nhấn "Forgot Password"
        private void NavigateToPasswordReset()
        {
            _navigationService.NavigateTo(AppViews.PasswordReset);
        }
    }
}