using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp UserDialogViewModel
    // + Tại sao cần sử dụng: Xử lý logic cho việc thêm mới hoặc chỉnh sửa thông tin người dùng
    // + Lớp này được gọi từ UserManagementViewModel khi mở dialog thêm/sửa người dùng
    // + Chức năng chính: Xác thực dữ liệu, xử lý lưu và cập nhật thông tin người dùng
    public class UserDialogViewModel : ViewModelBase
    {
        // 1. Dịch vụ cơ sở dữ liệu để truy vấn và cập nhật thông tin người dùng
        // 2. Được truyền vào từ constructor
        // 3. Dùng để tải và thao tác với dữ liệu người dùng
        private readonly DatabaseService _databaseService;
        
        // 1. Tham chiếu đến PasswordBox và ConfirmPasswordBox
        // 2. Không thể binding trực tiếp vì lý do bảo mật
        // 3. Dùng để truy cập giá trị mật khẩu khi lưu
        private PasswordBox _passwordBox;
        private PasswordBox _confirmPasswordBox;
        
        // 1. Người dùng hiện tại đang được chỉnh sửa
        // 2. Null nếu đang thêm mới người dùng
        // 3. Được truyền vào từ UserManagementViewModel
        private User _currentUser;
        
        // 1. Sự kiện yêu cầu đóng dialog
        // 2. Được gọi khi người dùng lưu thành công hoặc hủy
        // 3. Truyền kết quả về cho View thông qua DialogResult
        public event Action<bool> RequestClose;
        
        // 1. Tiêu đề của dialog
        // 2. Thay đổi dựa trên việc thêm mới hay chỉnh sửa
        // 3. Hiển thị trên thanh tiêu đề của dialog
        private string _dialogTitle;
        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetProperty(ref _dialogTitle, value);
        }
        
        // 1. Trạng thái xử lý của dialog
        // 2. True khi đang xử lý tác vụ bất đồng bộ (như kiểm tra hoặc lưu)
        // 3. Dùng để hiển thị giao diện loading
        private bool _isProcessing;
        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }
        
        // 1. Cờ đánh dấu đây là người dùng mới hay không
        // 2. Ảnh hưởng đến các trường có thể chỉnh sửa và logic xác thực
        // 3. True khi thêm mới, False khi chỉnh sửa
        private bool _isNewUser;
        public bool IsNewUser
        {
            get => _isNewUser;
            set => SetProperty(ref _isNewUser, value);
        }
        
        // 1. Tên đăng nhập của người dùng
        // 2. Bắt buộc và phải là duy nhất trong hệ thống
        // 3. Không thể thay đổi khi chỉnh sửa người dùng
        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    ValidateUsername();
                }
            }
        }
        
        // 1. Lỗi tên đăng nhập
        // 2. Hiển thị khi tên đăng nhập không hợp lệ
        // 3. Rỗng khi không có lỗi
        private string _usernameError = string.Empty;
        public string UsernameError
        {
            get => _usernameError;
            set => SetProperty(ref _usernameError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi tên đăng nhập hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasUsernameError => !string.IsNullOrEmpty(UsernameError);
        
        // 1. Email của người dùng
        // 2. Bắt buộc và phải là duy nhất trong hệ thống
        // 3. Phải có định dạng email hợp lệ
        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    ValidateEmail();
                }
            }
        }
        
        // 1. Lỗi email
        // 2. Hiển thị khi email không hợp lệ
        // 3. Rỗng khi không có lỗi
        private string _emailError = string.Empty;
        public string EmailError
        {
            get => _emailError;
            set => SetProperty(ref _emailError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi email hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasEmailError => !string.IsNullOrEmpty(EmailError);
        
        // 1. Tên đầy đủ của người dùng
        // 2. Bắt buộc và hiển thị trong giao diện
        // 3. Hiển thị thân thiện hơn tên đăng nhập
        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set
            {
                if (SetProperty(ref _fullName, value))
                {
                    ValidateFullName();
                }
            }
        }
        
        // 1. Lỗi tên đầy đủ
        // 2. Hiển thị khi tên đầy đủ không hợp lệ
        // 3. Rỗng khi không có lỗi
        private string _fullNameError = string.Empty;
        public string FullNameError
        {
            get => _fullNameError;
            set => SetProperty(ref _fullNameError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi tên đầy đủ hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasFullNameError => !string.IsNullOrEmpty(FullNameError);
        
        // 1. Danh sách vai trò có sẵn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Gồm các vai trò trong hệ thống
        public ObservableCollection<string> AvailableRoles { get; } = new ObservableCollection<string> 
        { 
            "Admin", 
            "Teacher", 
            "Student" 
        };
        
        // 1. Vai trò được chọn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Mặc định là Student nếu thêm mới
        private string _selectedRole = "Student";
        public string SelectedRole
        {
            get => _selectedRole;
            set => SetProperty(ref _selectedRole, value);
        }
        
        // 1. Trạng thái hoạt động của tài khoản
        // 2. Binding đến CheckBox trong giao diện
        // 3. Mặc định là true (hoạt động)
        private bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
        
        // 1. Lỗi mật khẩu
        // 2. Hiển thị khi mật khẩu không hợp lệ
        // 3. Rỗng khi không có lỗi
        private string _passwordError = string.Empty;
        public string PasswordError
        {
            get => _passwordError;
            set
            {
                if (SetProperty(ref _passwordError, value))
                {
                    OnPropertyChanged(nameof(HasPasswordError));
                }
            }
        }
        
        // 1. Cờ đánh dấu có lỗi mật khẩu hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasPasswordError => !string.IsNullOrEmpty(PasswordError);
        
        // 1. Lỗi xác nhận mật khẩu
        // 2. Hiển thị khi xác nhận mật khẩu không khớp
        // 3. Rỗng khi không có lỗi
        private string _confirmPasswordError = string.Empty;
        public string ConfirmPasswordError
        {
            get => _confirmPasswordError;
            set
            {
                if (SetProperty(ref _confirmPasswordError, value))
                {
                    OnPropertyChanged(nameof(HasConfirmPasswordError));
                }
            }
        }
        
        // 1. Cờ đánh dấu có lỗi xác nhận mật khẩu hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasConfirmPasswordError => !string.IsNullOrEmpty(ConfirmPasswordError);
        
        // 1. Lệnh lưu người dùng
        // 2. Binding đến nút "Lưu" trong giao diện
        // 3. Thực hiện xác thực và lưu dữ liệu
        public ICommand SaveCommand { get; }
        
        // 1. Lệnh hủy thao tác
        // 2. Binding đến nút "Hủy" trong giao diện
        // 3. Đóng dialog mà không lưu
        public ICommand CancelCommand { get; }

        // 1. Constructor cho người dùng mới
        // 2. Khởi tạo ViewModel để thêm người dùng mới
        // 3. Thiết lập tiêu đề và trạng thái phù hợp
        public UserDialogViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _isNewUser = true;
            DialogTitle = "Add New User";
            
            SaveCommand = new RelayCommand(async param => await SaveUserAsync());
            CancelCommand = new RelayCommand(param => CloseDialog(false));
        }
        
        // 1. Constructor cho chỉnh sửa người dùng
        // 2. Khởi tạo ViewModel với thông tin người dùng hiện có
        // 3. Điền dữ liệu từ người dùng được chỉnh sửa
        public UserDialogViewModel(DatabaseService databaseService, User user)
        {
            _databaseService = databaseService;
            _currentUser = user;
            _isNewUser = false;
            DialogTitle = "Edit User";
            
            // Populate fields from existing user
            Username = user.Username;
            Email = user.Email;
            FullName = user.FullName;
            SelectedRole = user.Role;
            IsActive = user.IsActive;
            
            SaveCommand = new RelayCommand(async param => await SaveUserAsync());
            CancelCommand = new RelayCommand(param => CloseDialog(false));
        }
        
        // 1. Phương thức thiết lập tham chiếu đến các PasswordBox
        // 2. Được gọi từ code-behind của View
        // 3. Cần thiết vì không thể binding trực tiếp với PasswordBox
        public void SetPasswordBoxes(PasswordBox passwordBox, PasswordBox confirmPasswordBox)
        {
            _passwordBox = passwordBox;
            _confirmPasswordBox = confirmPasswordBox;
        }
        
        // 1. Phương thức xác thực tên đăng nhập
        // 2. Kiểm tra tính hợp lệ và tính duy nhất của tên đăng nhập
        // 3. Cập nhật lỗi nếu tên đăng nhập không hợp lệ
        private void ValidateUsername()
        {
            UsernameError = string.Empty;
            
            if (string.IsNullOrWhiteSpace(Username))
            {
                UsernameError = "Username is required";
                return;
            }
            
            if (Username.Length < 3)
            {
                UsernameError = "Username must be at least 3 characters";
                return;
            }
            
            if (!Regex.IsMatch(Username, @"^[a-zA-Z0-9_]+$"))
            {
                UsernameError = "Username can only contain letters, numbers and underscore";
                return;
            }
            
            OnPropertyChanged(nameof(HasUsernameError));
        }
        
        // 1. Phương thức xác thực email
        // 2. Kiểm tra định dạng và tính duy nhất của email
        // 3. Cập nhật lỗi nếu email không hợp lệ
        private void ValidateEmail()
        {
            EmailError = string.Empty;
            
            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "Email is required";
                return;
            }
            
            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                EmailError = "Please enter a valid email address";
                return;
            }
            
            OnPropertyChanged(nameof(HasEmailError));
        }
        
        // 1. Phương thức xác thực tên đầy đủ
        // 2. Kiểm tra tính hợp lệ của tên đầy đủ
        // 3. Cập nhật lỗi nếu tên đầy đủ không hợp lệ
        private void ValidateFullName()
        {
            FullNameError = string.Empty;
            
            if (string.IsNullOrWhiteSpace(FullName))
            {
                FullNameError = "Full name is required";
                return;
            }
            
            OnPropertyChanged(nameof(HasFullNameError));
        }
        
        // 1. Phương thức xác thực mật khẩu
        // 2. Kiểm tra tính hợp lệ của mật khẩu và sự khớp nhau
        // 3. Cập nhật lỗi nếu mật khẩu không hợp lệ
        private bool ValidatePasswords()
        {
            PasswordError = string.Empty;
            ConfirmPasswordError = string.Empty;
            bool isValid = true;
            
            string password = _passwordBox?.Password ?? string.Empty;
            string confirmPassword = _confirmPasswordBox?.Password ?? string.Empty;
            
            // For existing users, empty password means keep current password
            if (!IsNewUser && string.IsNullOrEmpty(password))
            {
                return true;
            }
            
            if (IsNewUser && string.IsNullOrEmpty(password))
            {
                PasswordError = "Password is required for new users";
                isValid = false;
            }
            else if (!string.IsNullOrEmpty(password) && password.Length < 6)
            {
                PasswordError = "Password must be at least 6 characters";
                isValid = false;
            }
            
            if (!string.IsNullOrEmpty(password) && password != confirmPassword)
            {
                ConfirmPasswordError = "Passwords do not match";
                isValid = false;
            }
            
            OnPropertyChanged(nameof(HasPasswordError));
            OnPropertyChanged(nameof(HasConfirmPasswordError));
            
            return isValid;
        }
        
        // 1. Phương thức kiểm tra tính duy nhất của tên đăng nhập và email
        // 2. Truy vấn cơ sở dữ liệu để kiểm tra
        // 3. Trả về true nếu duy nhất, false nếu đã tồn tại
        private async Task<bool> CheckUsernameDuplicateAsync()
        {
            if (!IsNewUser) return true; // Skip check for existing users (username can't be changed)
            
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            var parameters = new Dictionary<string, object>
            {
                { "@Username", Username }
            };
            
            int count = Convert.ToInt32(await _databaseService.ExecuteScalarAsync(query, parameters));
            if (count > 0)
            {
                UsernameError = "Username already exists";
                OnPropertyChanged(nameof(HasUsernameError));
                return false;
            }
            
            return true;
        }
        
        // 1. Phương thức kiểm tra tính duy nhất của email
        // 2. Truy vấn cơ sở dữ liệu để kiểm tra
        // 3. Trả về true nếu duy nhất, false nếu đã tồn tại
        private async Task<bool> CheckEmailDuplicateAsync()
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
            var parameters = new Dictionary<string, object>
            {
                { "@Email", Email }
            };
            
            // For existing users, exclude current user from check
            if (!IsNewUser)
            {
                query += " AND UserID != @UserID";
                parameters.Add("@UserID", _currentUser.UserID);
            }
            
            int count = Convert.ToInt32(await _databaseService.ExecuteScalarAsync(query, parameters));
            if (count > 0)
            {
                EmailError = "Email already exists";
                OnPropertyChanged(nameof(HasEmailError));
                return false;
            }
            
            return true;
        }
        
        // 1. Phương thức lưu người dùng
        // 2. Xác thực dữ liệu trước khi lưu
        // 3. Thêm mới hoặc cập nhật người dùng tùy thuộc vào IsNewUser
        private async Task SaveUserAsync()
        {
            // Validate all fields
            ValidateUsername();
            ValidateEmail();
            ValidateFullName();
            bool passwordsValid = ValidatePasswords();
            
            if (HasUsernameError || HasEmailError || HasFullNameError || !passwordsValid)
            {
                return;
            }
            
            IsProcessing = true;
            
            // Check duplicates
            bool uniqueUsername = await CheckUsernameDuplicateAsync();
            bool uniqueEmail = await CheckEmailDuplicateAsync();
            
            if (!uniqueUsername || !uniqueEmail)
            {
                IsProcessing = false;
                return;
            }
            
            try
            {
                if (IsNewUser)
                {
                    await CreateUserAsync();
                }
                else
                {
                    await UpdateUserAsync();
                }
                
                CloseDialog(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }
        
        // 1. Phương thức tạo người dùng mới
        // 2. Tạo hash mật khẩu và salt
        // 3. Thêm người dùng mới vào cơ sở dữ liệu
        private async Task CreateUserAsync()
        {
            // Create password hash and salt
            byte[] passwordSalt;
            byte[] passwordHash;
            
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(_passwordBox.Password));
            }
            
            string query = @"
                INSERT INTO Users (Username, PasswordHash, PasswordSalt, Email, Role, IsActive, CreatedDate, LastLoginDate)
                VALUES (@Username, @PasswordHash, @PasswordSalt, @Email, @Role, @IsActive, GETDATE(), NULL)";
            
            var parameters = new Dictionary<string, object>
            {
                { "@Username", Username },
                { "@PasswordHash", passwordHash },
                { "@PasswordSalt", passwordSalt },
                { "@Email", Email },
                { "@Role", SelectedRole },
                { "@IsActive", IsActive }
            };
            
            await _databaseService.ExecuteNonQueryAsync(query, parameters);
        }
        
        // 1. Phương thức cập nhật người dùng
        // 2. Cập nhật thông tin người dùng trong cơ sở dữ liệu
        // 3. Xử lý cập nhật mật khẩu nếu được nhập mới
        private async Task UpdateUserAsync()
        {
            string query;
            var parameters = new Dictionary<string, object>();
            
            // Check if password needs to be updated
            if (!string.IsNullOrEmpty(_passwordBox.Password))
            {
                // Create new password hash and salt
                byte[] passwordSalt;
                byte[] passwordHash;
                
                using (var hmac = new HMACSHA512())
                {
                    passwordSalt = hmac.Key;
                    passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(_passwordBox.Password));
                }
                
                query = @"
                    UPDATE Users
                    SET Email = @Email, 
                        PasswordHash = @PasswordHash, 
                        PasswordSalt = @PasswordSalt, 
                        Role = @Role, 
                        IsActive = @IsActive
                    WHERE UserID = @UserID";
                
                parameters.Add("@PasswordHash", passwordHash);
                parameters.Add("@PasswordSalt", passwordSalt);
            }
            else
            {
                // Update without changing password
                query = @"
                    UPDATE Users
                    SET Email = @Email,  
                        Role = @Role, 
                        IsActive = @IsActive
                    WHERE UserID = @UserID";
            }
            
            parameters.Add("@Email", Email);
            parameters.Add("@Role", SelectedRole);
            parameters.Add("@IsActive", IsActive);
            parameters.Add("@UserID", _currentUser.UserID);
            
            await _databaseService.ExecuteNonQueryAsync(query, parameters);
            
            // Update FullName
            if (!string.IsNullOrEmpty(FullName))
            {
                // Use a separate query to update the appropriate table based on role
                if (SelectedRole == "Student")
                {
                    string[] names = FullName.Split(new[] { ' ' }, 2);
                    string firstName = names[0];
                    string lastName = names.Length > 1 ? names[1] : "";
                    
                    string updateQuery = @"
                        UPDATE Students
                        SET FirstName = @FirstName, LastName = @LastName
                        WHERE UserID = @UserID";
                    
                    var updateParams = new Dictionary<string, object>
                    {
                        { "@FirstName", firstName },
                        { "@LastName", lastName },
                        { "@UserID", _currentUser.UserID }
                    };
                    
                    await _databaseService.ExecuteNonQueryAsync(updateQuery, updateParams);
                }
                else if (SelectedRole == "Teacher")
                {
                    string[] names = FullName.Split(new[] { ' ' }, 2);
                    string firstName = names[0];
                    string lastName = names.Length > 1 ? names[1] : "";
                    
                    string updateQuery = @"
                        UPDATE Teachers
                        SET FirstName = @FirstName, LastName = @LastName
                        WHERE UserID = @UserID";
                    
                    var updateParams = new Dictionary<string, object>
                    {
                        { "@FirstName", firstName },
                        { "@LastName", lastName },
                        { "@UserID", _currentUser.UserID }
                    };
                    
                    await _databaseService.ExecuteNonQueryAsync(updateQuery, updateParams);
                }
            }
        }
        
        // 1. Phương thức đóng dialog
        // 2. Gọi sự kiện RequestClose để thông báo kết quả
        // 3. true nếu lưu thành công, false nếu hủy
        private void CloseDialog(bool result)
        {
            RequestClose?.Invoke(result);
        }
    }
}
