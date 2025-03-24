using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Services;
using System;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    public class StudentDashboardViewModel : ViewModelBase
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
        public ICommand NavigateToViewAssignmentsCommand { get; }
        public ICommand NavigateToSubmissionManagementCommand { get; }

        public StudentDashboardViewModel(AuthenticationService authService, Services.NavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;

            WelcomeMessage = $"Welcome, {_authService.CurrentUser?.Username ?? "Student"}!";

            LogoutCommand = new RelayCommand(param => Logout());
            NavigateToViewAssignmentsCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.ViewAssignments));
            NavigateToSubmissionManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.SubmissionManagement));
        }

        private void Logout()
        {
            _authService.Logout();
            _navigationService.NavigateTo(AppViews.Login);
        }
    }
}
