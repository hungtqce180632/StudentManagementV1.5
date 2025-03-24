using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    public class PasswordResetViewModel : ViewModelBase
    {
        private readonly AuthenticationService _authService;
        private readonly EmailService _emailService;
        private readonly NavigationService _navigationService;

        private string _email = string.Empty;
        private string _token = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _message = string.Empty;
        private bool _isLoading;
        private bool _isTokenRequested;
        private bool _isSuccess;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Token
        {
            get => _token;
            set => SetProperty(ref _token, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsTokenRequested
        {
            get => _isTokenRequested;
            set => SetProperty(ref _isTokenRequested, value);
        }

        public bool IsSuccess
        {
            get => _isSuccess;
            set => SetProperty(ref _isSuccess, value);
        }

        public ICommand RequestTokenCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand BackToLoginCommand { get; }

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

        private void BackToLogin()
        {
            _navigationService.NavigateTo(AppViews.Login);
        }
    }
}