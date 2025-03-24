using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly AuthenticationService _authService;
        private readonly NavigationService _navigationService;

        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading;

        // Add an event to notify when login is successful
        public event EventHandler? LoginSuccessful;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        public LoginViewModel(AuthenticationService authService, NavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;

            LoginCommand = new RelayCommand(async param => await Login(), 
                param => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !IsLoading);
            
            ForgotPasswordCommand = new RelayCommand(param => NavigateToPasswordReset());
        }

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

        private void NavigateToPasswordReset()
        {
            _navigationService.NavigateTo(AppViews.PasswordReset);
        }
    }
}