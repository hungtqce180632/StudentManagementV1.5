using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Services;
using System;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    public class TeacherDashboardViewModel : ViewModelBase
    {
        private readonly AuthenticationService _authService;
        private readonly Services.NavigationService _navigationService;

        private string _welcomeMessage = string.Empty;

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        public ICommand LogoutCommand { get; }
        public ICommand NavigateToAssignmentManagementCommand { get; }

        public TeacherDashboardViewModel(AuthenticationService authService, Services.NavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;

            WelcomeMessage = $"Welcome, {_authService.CurrentUser?.Username ?? "Teacher"}!";

            LogoutCommand = new RelayCommand(param => Logout());
            NavigateToAssignmentManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.AssignmentManagement));
        }

        private void Logout()
        {
            _authService.Logout();
            _navigationService.NavigateTo(AppViews.Login);
        }
    }
}
