using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
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
        // 3. Có thể là "All", "Admin", "Teacher" hoặc "Student"
        private string _selectedRole = "All";
        
        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến indicator trong UI
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        private bool _isLoading;
        
        // 1. Người dùng được chọn trong danh sách
        // 2. Binding đến SelectedItem của DataGrid/ListView
        // 3. Dùng cho các thao tác sửa, xóa hoặc hiển thị chi tiết
        private User _selectedUser;
        
        // 1. Tùy chọn hiển thị người dùng không hoạt động
        // 2. Binding đến CheckBox trong UI
        // 3. Khi thay đổi sẽ lọc danh sách người dùng
        private bool _showInactiveUsers = false;
        
        // 1. Danh sách các vai trò có thể lọc
        // 2. Binding đến ItemsSource của ComboBox lọc vai trò
        // 3. Được khởi tạo với các vai trò trong hệ thống
        private ObservableCollection<string> _availableRoles = new ObservableCollection<string>();

        // 1. Danh sách người dùng hiển thị trong UI
        // 2. Binding đến DataGrid hoặc ListView
        // 3. Được tải từ cơ sở dữ liệu và lọc theo các điều kiện
        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        // 1. Từ khóa tìm kiếm
        // 2. Binding đến TextBox tìm kiếm
        // 3. Khi thay đổi sẽ gọi LoadUsersAsync để cập nhật danh sách
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

        // 1. Vai trò được chọn để lọc
        // 2. Binding đến ComboBox lọc theo vai trò
        // 3. Khi thay đổi sẽ gọi LoadUsersAsync để cập nhật danh sách
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

        // 1. Tùy chọn hiển thị người dùng không hoạt động
        // 2. Binding đến CheckBox trong UI
        // 3. Khi thay đổi sẽ gọi LoadUsersAsync để cập nhật danh sách
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

        // 1. Danh sách các vai trò có thể lọc
        // 2. Binding đến ItemsSource của ComboBox lọc vai trò
        // 3. Được khởi tạo với các vai trò trong hệ thống
        public ObservableCollection<string> AvailableRoles
        {
            get => _availableRoles;
            set => SetProperty(ref _availableRoles, value);
        }

        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến indicator trong UI
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // 1. Người dùng được chọn trong danh sách
        // 2. Binding đến SelectedItem của DataGrid/ListView
        // 3. Dùng cho các thao tác sửa, xóa hoặc hiển thị chi tiết
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

            // Load users when ViewModel is created
            LoadUsersAsync();
        }

        // 1. Phương thức tải danh sách người dùng
        // 2. Truy vấn cơ sở dữ liệu với các điều kiện lọc
        // 3. Cập nhật danh sách Users với kết quả tìm kiếm
        private async void LoadUsersAsync()
        {
            try
            {
                IsLoading = true;
                
                string query = BuildUserQuery();
                var parameters = BuildQueryParameters();
                DataTable result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                // Update UI on the dispatcher thread to avoid collection modified exception
                Application.Current.Dispatcher.Invoke(() => {
                    Users.Clear();
                    PopulateUsersFromDataTable(result);
                });
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
        
        // 1. Phương thức xây dựng câu truy vấn SQL
        // 2. Tạo câu truy vấn dựa trên điều kiện tìm kiếm và bộ lọc
        // 3. Trả về chuỗi truy vấn SQL hoàn chỉnh
        private string BuildUserQuery()
        {
            string query = @"
                SELECT 
                    u.UserID, 
                    u.Username, 
                    u.Email, 
                    u.Role, 
                    u.IsActive, 
                    u.CreatedDate, 
                    u.LastLoginDate,
                    CASE 
                        WHEN u.Role = 'Teacher' THEN (SELECT FirstName + ' ' + LastName FROM Teachers t WHERE t.UserID = u.UserID)
                        WHEN u.Role = 'Student' THEN (SELECT FirstName + ' ' + LastName FROM Students s WHERE s.UserID = u.UserID)
                        ELSE u.Username
                    END AS FullName
                FROM Users u
                WHERE 1=1";
            
            // Apply role filter
            if (_selectedRole != "All")
            {
                query += " AND u.Role = @Role";
            }
            
            // Apply active status filter
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
                    OR (u.Role = 'Teacher' AND EXISTS (
                        SELECT 1 FROM Teachers t 
                        WHERE t.UserID = u.UserID 
                        AND (t.FirstName LIKE @Search OR t.LastName LIKE @Search)
                    ))
                    OR (u.Role = 'Student' AND EXISTS (
                        SELECT 1 FROM Students s 
                        WHERE s.UserID = u.UserID 
                        AND (s.FirstName LIKE @Search OR s.LastName LIKE @Search)
                    ))
                )";
            }
            
            query += " ORDER BY u.Username";
            
            return query;
        }
        
        // 1. Phương thức tạo tham số cho truy vấn
        // 2. Tạo dictionary chứa các tham số truy vấn
        // 3. Trả về dictionary với tham số vai trò và tìm kiếm (nếu có)
        private Dictionary<string, object> BuildQueryParameters()
        {
            var parameters = new Dictionary<string, object>();
            
            if (_selectedRole != "All")
            {
                parameters["@Role"] = _selectedRole;
            }
            
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                parameters["@Search"] = $"%{_searchText}%";
            }
            
            return parameters;
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
        // 2. Hiển thị thông báo chức năng sẽ được triển khai trong tương lai
        // 3. Sẽ mở hộp thoại thực sự trong triển khai tương lai
        private void OpenAddUserDialog()
        {
            // Show dialog to add new user
            MessageBox.Show("Add user functionality will be implemented here", "Information", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 1. Phương thức mở hộp thoại sửa người dùng
        // 2. Hiển thị thông báo chức năng sẽ được triển khai trong tương lai
        // 3. Sẽ mở hộp thoại sửa với thông tin người dùng đã chọn trong triển khai tương lai
        private void OpenEditUserDialog(User user)
        {
            if (user == null) return;
            
            // Show dialog to edit user
            MessageBox.Show($"Edit user {user.Username} functionality will be implemented here", 
                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
