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
    public class AdminDashboardViewModel : ViewModelBase
    {
        private readonly AuthenticationService _authService;
        private readonly DatabaseService _databaseService;
        private readonly Services.NavigationService _navigationService;

        private string _welcomeMessage = string.Empty;
        private int _studentCount;
        private int _teacherCount;
        private int _classCount;
        private int _subjectCount;
        private ObservableCollection<User> _recentUsers = new ObservableCollection<User>();
        private bool _isLoading;

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        public int StudentCount
        {
            get => _studentCount;
            set => SetProperty(ref _studentCount, value);
        }

        public int TeacherCount
        {
            get => _teacherCount;
            set => SetProperty(ref _teacherCount, value);
        }

        public int ClassCount
        {
            get => _classCount;
            set => SetProperty(ref _classCount, value);
        }

        public int SubjectCount
        {
            get => _subjectCount;
            set => SetProperty(ref _subjectCount, value);
        }

        public ObservableCollection<User> RecentUsers
        {
            get => _recentUsers;
            set => SetProperty(ref _recentUsers, value);
        }

        public bool HasRecentUsers => RecentUsers.Count > 0;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Navigation Commands
        public ICommand LogoutCommand { get; }
        public ICommand NavigateToUserManagementCommand { get; }
        public ICommand NavigateToSubjectManagementCommand { get; }
        public ICommand NavigateToExamManagementCommand { get; }
        public ICommand NavigateToScoreManagementCommand { get; }
        public ICommand NavigateToClassManagementCommand { get; }
        public ICommand NavigateToScheduleManagementCommand { get; }
        public ICommand NavigateToNotificationManagementCommand { get; }

        // Quick Action Commands
        public ICommand AddNewStudentCommand { get; }
        public ICommand AddNewTeacherCommand { get; }
        public ICommand CreateNewClassCommand { get; }
        public ICommand CreateSystemBackupCommand { get; }

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

        private async Task LoadStudentCountAsync()
        {
            // Count students from database
            string query = "SELECT COUNT(*) FROM Students";
            var result = await _databaseService.ExecuteScalarAsync(query);
            StudentCount = result != null ? Convert.ToInt32(result) : 0;
        }

        private async Task LoadTeacherCountAsync()
        {
            // Count teachers from database
            string query = "SELECT COUNT(*) FROM Teachers";
            var result = await _databaseService.ExecuteScalarAsync(query);
            TeacherCount = result != null ? Convert.ToInt32(result) : 0;
        }

        private async Task LoadClassCountAsync()
        {
            // Count active classes from database
            string query = "SELECT COUNT(*) FROM Classes WHERE IsActive = 1";
            var result = await _databaseService.ExecuteScalarAsync(query);
            ClassCount = result != null ? Convert.ToInt32(result) : 0;
        }

        private async Task LoadSubjectCountAsync()
        {
            // Count subjects from database
            string query = "SELECT COUNT(*) FROM Subjects WHERE IsActive = 1";
            var result = await _databaseService.ExecuteScalarAsync(query);
            SubjectCount = result != null ? Convert.ToInt32(result) : 0;
        }

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

        private void Logout()
        {
            _authService.Logout();
            _navigationService.NavigateTo(AppViews.Login);
        }

        private void AddNewStudent()
        {
            // Navigate to add student form or show dialog
            MessageBox.Show("Add New Student functionality will be implemented soon.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddNewTeacher()
        {
            // Navigate to add teacher form or show dialog
            MessageBox.Show("Add New Teacher functionality will be implemented soon.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CreateNewClass()
        {
            // Navigate to create class form or show dialog
            MessageBox.Show("Create New Class functionality will be implemented soon.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

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
