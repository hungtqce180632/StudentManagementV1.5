using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
using System;
using System.Collections.ObjectModel;
using System.Data;
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
        private User _selectedUser;

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

        public User SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
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
            EditUserCommand = new RelayCommand(param => OpenEditUserDialog(param as User), param => param != null);
            DeleteUserCommand = new RelayCommand(async param => await DeleteUserAsync(param as User), param => param != null);
            BackCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.AdminDashboard));

            LoadUsersAsync();
        }

        private async void LoadUsersAsync()
        {
            try
            {
                IsLoading = true;
                Users.Clear();

                string query = "SELECT * FROM Users WHERE 1=1";
                var parameters = new Dictionary<string, object>();

                if (_selectedRole != "All")
                {
                    query += " AND Role = @Role";
                    parameters["@Role"] = _selectedRole;
                }

                if (!string.IsNullOrWhiteSpace(_searchText))
                {
                    query += " AND (Username LIKE @Search OR Email LIKE @Search)";
                    parameters["@Search"] = $"%{_searchText}%";
                }

                query += " ORDER BY Username";

                DataTable result = await _databaseService.ExecuteQueryAsync(query, parameters);

                // Process the results
                foreach (DataRow row in result.Rows)
                {
                    Users.Add(new User
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
            // Show dialog to add new user
            MessageBox.Show("Add user functionality will be implemented here", "Information", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenEditUserDialog(User user)
        {
            if (user == null) return;
            
            // Show dialog to edit user
            MessageBox.Show($"Edit user {user.Username} functionality will be implemented here", 
                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task DeleteUserAsync(User user)
        {
            if (user == null) return;

            // Confirm deletion
            var result = MessageBox.Show($"Are you sure you want to delete {user.Username}?", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    IsLoading = true;

                    // Check if this is the currently logged-in user
                    if (user.UserID == _authService.CurrentUser?.UserID)
                    {
                        MessageBox.Show("You cannot delete your own account while logged in.", 
                            "Operation Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Instead of actually deleting, we'll set IsActive = 0
                    string query = $"UPDATE Users SET IsActive = 0 WHERE UserID = {user.UserID}";
                    await _databaseService.ExecuteNonQueryAsync(query);

                    // Refresh the list
                    LoadUsersAsync();

                    MessageBox.Show($"User {user.Username} has been deactivated.", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting user: {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }
    }
}
