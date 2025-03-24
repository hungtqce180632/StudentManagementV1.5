using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Services;
using System;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    public class AdminDashboardViewModel : ViewModelBase
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
        public ICommand NavigateToUserManagementCommand { get; }
        public ICommand NavigateToSubjectManagementCommand { get; }
        public ICommand NavigateToExamManagementCommand { get; }
        public ICommand NavigateToScoreManagementCommand { get; }
        public ICommand NavigateToClassManagementCommand { get; }
        public ICommand NavigateToScheduleManagementCommand { get; }
        public ICommand NavigateToNotificationManagementCommand { get; }

        public AdminDashboardViewModel(AuthenticationService authService, Services.NavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;

            WelcomeMessage = $"Welcome, {_authService.CurrentUser?.Username ?? "Admin"}!";

            LogoutCommand = new RelayCommand(param => Logout());
            
            // Initialize navigation commands
            NavigateToUserManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.UserManagement));
            NavigateToSubjectManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.SubjectManagement));
            NavigateToExamManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.ExamManagement));
            NavigateToScoreManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.ScoreManagement));
            NavigateToClassManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.ClassManagement));
            NavigateToScheduleManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.ScheduleManagement));
            NavigateToNotificationManagementCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.NotificationManagement));
        }

        private void Logout()
        {
            _authService.Logout();
            _navigationService.NavigateTo(AppViews.Login);
        }
    }
}
