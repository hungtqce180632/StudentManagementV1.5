using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp PasswordResetViewModel
    // + Tại sao cần sử dụng: Cung cấp chức năng đặt lại mật khẩu cho người dùng quên mật khẩu
    // + Lớp này được gọi từ màn hình đặt lại mật khẩu
    // + Chức năng chính: Xử lý việc gửi yêu cầu token và đặt lại mật khẩu mới
    public class PasswordResetViewModel : ViewModelBase
    {
        // 1. Dịch vụ xác thực để xác minh token và đặt lại mật khẩu
        // 2. Được truyền vào từ constructor
        // 3. Cung cấp các phương thức để yêu cầu và xác nhận đặt lại mật khẩu
        private readonly AuthenticationService _authService;
        
        // 1. Dịch vụ email để gửi token đặt lại mật khẩu
        // 2. Được truyền vào từ constructor
        // 3. Gửi token đến email của người dùng
        private readonly EmailService _emailService;
        
        // 1. Dịch vụ điều hướng
        // 2. Được truyền vào từ constructor
        // 3. Sử dụng để quay lại màn hình đăng nhập
        private readonly NavigationService _navigationService;

        // 1. Địa chỉ email người dùng nhập vào
        // 2. Binding đến TextBox trong giao diện
        // 3. Được sử dụng để tìm kiếm tài khoản và gửi token
        private string _email = string.Empty;
        
        // 1. Token đặt lại mật khẩu người dùng nhập vào
        // 2. Binding đến TextBox trong giao diện
        // 3. Được kiểm tra khi đặt lại mật khẩu
        private string _token = string.Empty;
        
        // 1. Mật khẩu mới người dùng nhập vào
        // 2. Binding đến PasswordBox trong giao diện
        // 3. Được cập nhật khi đặt lại mật khẩu thành công
        private string _newPassword = string.Empty;
        
        // 1. Nhập lại mật khẩu để xác nhận
        // 2. Binding đến PasswordBox trong giao diện
        // 3. Phải trùng khớp với mật khẩu mới
        private string _confirmPassword = string.Empty;
        
        // 1. Thông báo hiển thị cho người dùng
        // 2. Binding đến TextBlock trong giao diện
        // 3. Cập nhật để hiển thị thông báo lỗi hoặc thành công
        private string _message = string.Empty;
        
        // 1. Trạng thái đang tải
        // 2. Binding đến trạng thái hiển thị của ProgressBar
        // 3. Cập nhật khi bắt đầu và kết thúc quá trình xử lý
        private bool _isLoading;
        
        // 1. Đã yêu cầu token chưa
        // 2. Binding đến trạng thái hiển thị của các control
        // 3. Điều khiển luồng UI trong quá trình đặt lại mật khẩu
        private bool _isTokenRequested;
        
        // 1. Đặt lại mật khẩu thành công chưa
        // 2. Binding đến trạng thái hiển thị của các control
        // 3. Điều khiển luồng UI sau khi đặt lại mật khẩu
        private bool _isSuccess;

        // 1. Địa chỉ email người dùng nhập vào
        // 2. Binding đến TextBox trong giao diện
        // 3. Được sử dụng để tìm kiếm tài khoản và gửi token
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        // 1. Token đặt lại mật khẩu người dùng nhập vào
        // 2. Binding đến TextBox trong giao diện
        // 3. Được kiểm tra khi đặt lại mật khẩu
        public string Token
        {
            get => _token;
            set => SetProperty(ref _token, value);
        }

        // 1. Mật khẩu mới người dùng nhập vào
        // 2. Binding đến PasswordBox trong giao diện
        // 3. Được cập nhật khi đặt lại mật khẩu thành công
        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        // 1. Nhập lại mật khẩu để xác nhận
        // 2. Binding đến PasswordBox trong giao diện
        // 3. Phải trùng khớp với mật khẩu mới
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        // 1. Thông báo hiển thị cho người dùng
        // 2. Binding đến TextBlock trong giao diện
        // 3. Cập nhật để hiển thị thông báo lỗi hoặc thành công
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        // 1. Trạng thái đang tải
        // 2. Binding đến trạng thái hiển thị của ProgressBar
        // 3. Cập nhật khi bắt đầu và kết thúc quá trình xử lý
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // 1. Đã yêu cầu token chưa
        // 2. Binding đến trạng thái hiển thị của các control
        // 3. Điều khiển luồng UI trong quá trình đặt lại mật khẩu
        public bool IsTokenRequested
        {
            get => _isTokenRequested;
            set => SetProperty(ref _isTokenRequested, value);
        }

        // 1. Đặt lại mật khẩu thành công chưa
        // 2. Binding đến trạng thái hiển thị của các control
        // 3. Điều khiển luồng UI sau khi đặt lại mật khẩu
        public bool IsSuccess
        {
            get => _isSuccess;
            set => SetProperty(ref _isSuccess, value);
        }

        // 1. Lệnh yêu cầu token đặt lại mật khẩu
        // 2. Binding đến nút "Request Token" trong giao diện
        // 3. Gửi yêu cầu token đến email của người dùng
        public ICommand RequestTokenCommand { get; }
        
        // 1. Lệnh đặt lại mật khẩu
        // 2. Binding đến nút "Reset Password" trong giao diện
        // 3. Xác minh token và đặt lại mật khẩu mới
        public ICommand ResetPasswordCommand { get; }
        
        // 1. Lệnh quay lại màn hình đăng nhập
        // 2. Binding đến nút "Back to Login" trong giao diện
        // 3. Điều hướng người dùng đến màn hình đăng nhập
        public ICommand BackToLoginCommand { get; }

        // 1. Constructor của lớp
        // 2. Nhận các dịch vụ cần thiết
        // 3. Khởi tạo các lệnh và thiết lập trạng thái ban đầu
        public PasswordResetViewModel(AuthenticationService authService, EmailService emailService, NavigationService navigationService)
        {
            _authService = authService;
            _emailService = emailService;
            _navigationService = navigationService;

            RequestTokenCommand = new RelayCommand(async param => await RequestToken(), 
                param => !string.IsNullOrEmpty(Email) && !IsLoading && !IsTokenRequested);
            
            ResetPasswordCommand = new RelayCommand(async param => await ResetPassword(), 
                param => !string.IsNullOrEmpty(Token) && !string.IsNullOrEmpty(NewPassword) && 
                         NewPassword == ConfirmPassword && !IsLoading && IsTokenRequested);
            
            BackToLoginCommand = new RelayCommand(param => BackToLogin());
        }

        // 1. Phương thức yêu cầu token đặt lại mật khẩu
        // 2. Gửi yêu cầu đến AuthService và gửi token qua email
        // 3. Cập nhật UI để người dùng nhập token và mật khẩu mới
        private async Task RequestToken()
        {
            try
            {
                IsLoading = true;
                Message = string.Empty;

                // Generate a random token
                Random random = new Random();
                string generatedToken = random.Next(100000, 999999).ToString();

                bool result = await _authService.RequestPasswordResetAsync(Email);
                
                if (result)
                {
                    // Now send a real email with the token
                    await _emailService.SendPasswordResetEmailAsync(Email, generatedToken);
                    
                    Message = "A password reset token has been sent to your email.";
                    IsTokenRequested = true;
                }
                else
                {
                    Message = "Email not found.";
                }
            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // 1. Phương thức đặt lại mật khẩu
        // 2. Xác minh token và cập nhật mật khẩu mới
        // 3. Cập nhật UI để người dùng biết kết quả
        private async Task ResetPassword()
        {
            try
            {
                IsLoading = true;
                Message = string.Empty;

                if (NewPassword != ConfirmPassword)
                {
                    Message = "Passwords do not match.";
                    return;
                }

                bool result = await _authService.ResetPasswordAsync(Email, Token, NewPassword);
                
                if (result)
                {
                    Message = "Password has been reset successfully.";
                    IsSuccess = true;
                }
                else
                {
                    Message = "Invalid or expired token.";
                }
            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // 1. Phương thức quay lại màn hình đăng nhập
        // 2. Sử dụng NavigationService để chuyển màn hình
        // 3. Được gọi khi người dùng nhấn nút "Back to Login"
        private void BackToLogin()
        {
            _navigationService.NavigateTo(AppViews.Login);
        }
    }
}