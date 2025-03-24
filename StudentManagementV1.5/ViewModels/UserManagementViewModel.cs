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
    public class UserManagementViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthenticationService _authService;
        private readonly NavigationService _navigationService;

        private ObservableCollection<User> _users = new ObservableCollection<User>();
        private string _searchText = string.Empty;
        private string _selectedRole = "All";
        private bool _isLoading;

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    LoadUsersAsync();
                }
            }
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (SetProperty(ref _selectedRole, value))
                {
                    LoadUsersAsync();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand BackCommand { get; }

        public UserManagementViewModel(DatabaseService databaseService, AuthenticationService authService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _authService = authService;
            _navigationService = navigationService;

            AddUserCommand = new RelayCommand(param => OpenAddUserDialog());
            EditUserCommand = new RelayCommand(param => OpenEditUserDialog(param as User));
            DeleteUserCommand = new RelayCommand(param => DeleteUserAsync(param as User));
            BackCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.AdminDashboard));

            LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                IsLoading = true;
                
                // Implement database loading logic here
                // For now, add sample data
                Users = new ObservableCollection<User>
                {
                    new User { UserID = 1, Username = "admin", Email = "admin@school.com", Role = "Admin", IsActive = true, CreatedDate = DateTime.Now.AddDays(-30) },
                    new User { UserID = 2, Username = "teacher1", Email = "teacher1@school.com", Role = "Teacher", IsActive = true, CreatedDate = DateTime.Now.AddDays(-25) },
                    new User { UserID = 3, Username = "student1", Email = "student1@school.com", Role = "Student", IsActive = true, CreatedDate = DateTime.Now.AddDays(-20) }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OpenAddUserDialog()
        {
            // Implement dialog to add user
            MessageBox.Show("Add user functionality will be implemented here", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenEditUserDialog(User user)
        {
            if (user == null) return;
            
            // Implement dialog to edit user
            MessageBox.Show($"Edit user functionality for {user.Username} will be implemented here", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task DeleteUserAsync(User user)
        {
            if (user == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete {user.Username}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                // Implement user deletion here
                MessageBox.Show($"User {user.Username} would be deleted here", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
