using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
using StudentManagementV1._5.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp UserManagementViewModel
    // + Tại sao cần sử dụng: Quản lý và hiển thị danh sách người dùng trong hệ thống
    // + Lớp này được gọi từ màn hình quản lý người dùng (UserManagementView)
    // + Chức năng chính: Tìm kiếm, lọc, thêm, sửa và xóa người dùng
    public class UserManagementViewModel : ViewModelBase
    {
        // 1. Dịch vụ cơ sở dữ liệu để truy vấn và cập nhật thông tin người dùng
        // 2. Được truyền vào từ constructor
        // 3. Dùng để tải và thao tác với dữ liệu người dùng
        private readonly DatabaseService _databaseService;
        
        // 1. Dịch vụ xác thực người dùng
        // 2. Được truyền vào từ constructor
        // 3. Dùng để lấy thông tin người dùng hiện tại và kiểm tra quyền
        private readonly AuthenticationService _authService;
        
        // 1. Dịch vụ điều hướng
        // 2. Được truyền vào từ constructor
        // 3. Dùng để chuyển đổi giữa các màn hình
        private readonly NavigationService _navigationService;

        // 1. Danh sách người dùng hiển thị trong UI
        // 2. Binding đến DataGrid hoặc ListView 
        // 3. Được tải từ cơ sở dữ liệu và lọc theo các điều kiện
        private ObservableCollection<User> _users = new ObservableCollection<User>();
        
        // 1. Từ khóa tìm kiếm
        // 2. Binding đến TextBox tìm kiếm
        // 3. Dùng để lọc người dùng theo tên, email hoặc thông tin khác
        private string _searchText = string.Empty;
        
        // 1. Vai trò được chọn để lọc
        // 2. Binding đến ComboBox lọc theo vai trò
        // 3. Cho phép lọc người dùng theo vai trò cụ thể
        private string _selectedRole = "All";
        
        // 1. Cờ đánh dấu có hiển thị người dùng không hoạt động hay không
        // 2. Binding đến CheckBox hiển thị người dùng không hoạt động
        // 3. Ảnh hưởng đến câu truy vấn lấy dữ liệu
        private bool _showInactiveUsers;
        
        // 1. Cờ đánh dấu đang tải dữ liệu
        // 2. Binding đến overlay loading
        // 3. Hiển thị hiệu ứng loading khi đang tải dữ liệu
        private bool _isLoading;
        
        // 1. Người dùng được chọn trong danh sách
        // 2. Binding đến SelectedItem của DataGrid
        // 3. Được sử dụng cho các thao tác sửa và xóa
        private User _selectedUser;

        // 1. Danh sách vai trò có sẵn để lọc
        // 2. Binding đến ComboBox lọc theo vai trò
        // 3. Bao gồm tất cả vai trò trong hệ thống và lựa chọn "All"
        public ObservableCollection<string> AvailableRoles { get; } = new ObservableCollection<string>();

        // Các thuộc tính công khai với getter và setter
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

        public bool ShowInactiveUsers
        {
            get => _showInactiveUsers;
            set
            {
                if (SetProperty(ref _showInactiveUsers, value))
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

        // 1. Lệnh thêm người dùng mới
        // 2. Binding đến nút "Thêm người dùng" trong UI
        // 3. Khi được gọi, mở hộp thoại thêm người dùng
        public ICommand AddUserCommand { get; }
        
        // 1. Lệnh sửa người dùng
        // 2. Binding đến nút "Sửa" trong UI
        // 3. Khi được gọi, mở hộp thoại sửa người dùng với thông tin đã chọn
        public ICommand EditUserCommand { get; }
        
        // 1. Lệnh xóa người dùng
        // 2. Binding đến nút "Xóa" trong UI
        // 3. Khi được gọi, hiển thị xác nhận và xóa người dùng đã chọn
        public ICommand DeleteUserCommand { get; }
        
        // 1. Lệnh quay lại
        // 2. Binding đến nút "Quay lại" trong UI
        // 3. Khi được gọi, chuyển về màn hình dashboard
        public ICommand BackCommand { get; }

        // 1. Constructor của lớp
        // 2. Khởi tạo các tham số và thiết lập lệnh
        // 3. Tải dữ liệu ban đầu từ cơ sở dữ liệu
        public UserManagementViewModel(DatabaseService databaseService, AuthenticationService authService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _authService = authService;
            _navigationService = navigationService;

            AddUserCommand = new RelayCommand(param => OpenAddUserDialog());
            EditUserCommand = new RelayCommand(param => OpenEditUserDialog(param as User), param => param != null);
            DeleteUserCommand = new RelayCommand(async param => await DeleteUserAsync(param as User), param => param != null);
            BackCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.AdminDashboard));

            // Initialize available roles
            AvailableRoles.Add("All");
            AvailableRoles.Add("Admin");
            AvailableRoles.Add("Teacher");
            AvailableRoles.Add("Student");

            // Load initial data
            LoadUsersAsync();
        }

        // 1. Phương thức tải danh sách người dùng
        // 2. Truy vấn cơ sở dữ liệu với các điều kiện lọc
        // 3. Áp dụng bộ lọc theo tìm kiếm, vai trò và trạng thái
        private async void LoadUsersAsync()
        {
            IsLoading = true;
            Users.Clear();

            try
            {
                string query = @"
                    SELECT 
                        u.UserID, u.Username, u.Email, u.Role, u.IsActive, u.CreatedDate, u.LastLoginDate,
                        CASE 
                            WHEN u.Role = 'Student' THEN CONCAT(s.FirstName, ' ', s.LastName)
                            WHEN u.Role = 'Teacher' THEN CONCAT(t.FirstName, ' ', t.LastName)
                            ELSE u.Username
                        END as FullName
                    FROM Users u
                    LEFT JOIN Students s ON u.UserID = s.UserID AND u.Role = 'Student'
                    LEFT JOIN Teachers t ON u.UserID = t.UserID AND u.Role = 'Teacher'
                    WHERE 1=1";

                var parameters = new Dictionary<string, object>();

                // Apply role filter
                if (_selectedRole != "All")
                {
                    query += " AND u.Role = @Role";
                    parameters.Add("@Role", _selectedRole);
                }

                // Apply active filter
                if (!_showInactiveUsers)
                {
                    query += " AND u.IsActive = 1";
                }

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(_searchText))
                {
                    query += @" AND (
                        u.Username LIKE @Search 
                        OR u.Email LIKE @Search 
                        OR CONCAT(ISNULL(s.FirstName, ''), ' ', ISNULL(s.LastName, '')) LIKE @Search
                        OR CONCAT(ISNULL(t.FirstName, ''), ' ', ISNULL(t.LastName, '')) LIKE @Search
                    )";
                    parameters.Add("@Search", $"%{_searchText}%");
                }

                // Order by
                query += " ORDER BY u.UserID";

                DataTable result = await _databaseService.ExecuteQueryAsync(query, parameters);
                PopulateUsersFromDataTable(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // 1. Phương thức điền dữ liệu từ DataTable vào danh sách Users
        // 2. Chuyển đổi từng dòng dữ liệu thành đối tượng User
        // 3. Thêm đối tượng User vào danh sách Users
        private void PopulateUsersFromDataTable(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Users.Add(new User
                {
                    UserID = Convert.ToInt32(row["UserID"]),
                    Username = row["Username"].ToString() ?? string.Empty,
                    Email = row["Email"].ToString() ?? string.Empty,
                    Role = row["Role"].ToString() ?? string.Empty,
                    FullName = row["FullName"].ToString() ?? string.Empty,
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    LastLoginDate = row["LastLoginDate"] != DBNull.Value ? Convert.ToDateTime(row["LastLoginDate"]) : null
                });
            }
        }

        // 1. Phương thức mở hộp thoại thêm người dùng
        // 2. Tạo và hiển thị UserDialogView với UserDialogViewModel
        // 3. Tải lại danh sách người dùng nếu thêm thành công
        private void OpenAddUserDialog()
        {
            var viewModel = new UserDialogViewModel(_databaseService);
            var dialog = new UserDialogView(viewModel)
            {
                Owner = Application.Current.MainWindow
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                LoadUsersAsync();
            }
        }

        // 1. Phương thức mở hộp thoại sửa người dùng
        // 2. Tạo và hiển thị UserDialogView với UserDialogViewModel và dữ liệu người dùng
        // 3. Tải lại danh sách người dùng nếu cập nhật thành công
        private void OpenEditUserDialog(User user)
        {
            if (user == null) return;
            
            var viewModel = new UserDialogViewModel(_databaseService, user);
            var dialog = new UserDialogView(viewModel)
            {
                Owner = Application.Current.MainWindow
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                LoadUsersAsync();
            }
        }

        // 1. Phương thức xóa người dùng
        // 2. Hiển thị hộp thoại xác nhận trước khi xóa
        // 3. Nếu xác nhận, hủy kích hoạt người dùng thay vì xóa hoàn toàn
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
                            "Operation not allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Soft delete - set IsActive to false
                    string query = "UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";
                    var parameters = new Dictionary<string, object>
                    {
                        { "@UserID", user.UserID }
                    };

                    await _databaseService.ExecuteNonQueryAsync(query, parameters);
                    LoadUsersAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting user: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }
    }
}
