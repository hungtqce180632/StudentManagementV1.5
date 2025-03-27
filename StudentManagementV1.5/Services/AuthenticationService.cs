using StudentManagementV1._5.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementV1._5.Services
{
    // Lớp AuthenticationService
    // + Tại sao cần sử dụng: Quản lý xác thực người dùng và bảo mật trong hệ thống
    // + Lớp này được gọi từ các ViewModel đăng nhập và khôi phục mật khẩu
    // + Chức năng chính: Đăng nhập, đăng xuất và quản lý bảo mật tài khoản
    public class AuthenticationService
    {
        // 1. Tham chiếu đến dịch vụ cơ sở dữ liệu
        // 2. Dùng để truy vấn thông tin người dùng và xác thực
        // 3. Được truyền vào từ constructor
        private readonly DatabaseService _databaseService;
        
        // 1. Lưu trữ thông tin người dùng hiện tại đang đăng nhập
        // 2. Null khi chưa đăng nhập hoặc đã đăng xuất
        // 3. Được cập nhật khi đăng nhập thành công
        private User? _currentUser;

        // 1. Thuộc tính cung cấp thông tin người dùng hiện tại
        // 2. Được truy cập từ các ViewModel để xác định quyền truy cập
        // 3. Chỉ đọc để ngăn chặn việc thay đổi không mong muốn
        public User? CurrentUser => _currentUser;

        // 1. Constructor của AuthenticationService
        // 2. Nhận tham chiếu đến DatabaseService
        // 3. Khởi tạo service mà không cần đăng nhập
        public AuthenticationService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // 1. Phương thức đăng nhập người dùng
        // 2. Kiểm tra tên đăng nhập và mật khẩu
        // 3. Cập nhật thông tin người dùng hiện tại nếu đăng nhập thành công
        public async Task<bool> LoginAsync(string username, string password)
        {
            string query = "SELECT * FROM Users WHERE Username = @Username AND IsActive = 1";
            var parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            DataTable result = await _databaseService.ExecuteQueryAsync(query, parameters);

            if (result.Rows.Count == 0)
                return false;

            var userRow = result.Rows[0];
            bool isAuthenticated = false;
            string role = userRow["Role"].ToString() ?? string.Empty;

            // Special case for admin accounts which was created directly in SQL
            // Check either by username or role
            if (username.ToLower() == "admin" || 
                username.ToLower() == "administrator" || 
                role.ToLower() == "admin")
            {
                // The admin password in your database is hashed with SHA2_512 without a salt
                byte[] storedHash = (byte[])userRow["PasswordHash"];
                byte[] computedHash = null;
                
                using (SHA512 sha512 = SHA512.Create())
                {
                    computedHash = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
                }

                // Compare the byte arrays
                if (storedHash.Length == computedHash.Length)
                {
                    isAuthenticated = true;
                    for (int i = 0; i < storedHash.Length; i++)
                    {
                        if (storedHash[i] != computedHash[i])
                        {
                            isAuthenticated = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                // Regular accounts use HMACSHA512 with salt
                byte[] storedHash = (byte[])userRow["PasswordHash"];
                byte[] storedSalt = (byte[])userRow["PasswordSalt"];

                using var hmac = new HMACSHA512(storedSalt);
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                isAuthenticated = true;
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (i < storedHash.Length && computedHash[i] != storedHash[i])
                    {
                        isAuthenticated = false;
                        break;
                    }
                }
            }

            if (isAuthenticated)
            {
                // Update last login date
                string updateQuery = "UPDATE Users SET LastLoginDate = GETDATE() WHERE UserID = @UserID";
                var updateParams = new Dictionary<string, object>
                {
                    { "@UserID", userRow["UserID"] }
                };
                await _databaseService.ExecuteNonQueryAsync(updateQuery, updateParams);

                _currentUser = new User
                {
                    UserID = Convert.ToInt32(userRow["UserID"]),
                    Username = userRow["Username"].ToString() ?? string.Empty,
                    Email = userRow["Email"].ToString() ?? string.Empty,
                    Role = userRow["Role"].ToString() ?? string.Empty,
                    IsActive = Convert.ToBoolean(userRow["IsActive"]),
                    CreatedDate = Convert.ToDateTime(userRow["CreatedDate"]),
                    LastLoginDate = userRow["LastLoginDate"] != DBNull.Value ? Convert.ToDateTime(userRow["LastLoginDate"]) : null
                };

                return true;
            }

            return false;
        }

        // 1. Phương thức đặt lại mật khẩu
        // 2. Xác minh token và cập nhật mật khẩu mới
        // 3. Trả về true nếu đặt lại thành công, false nếu token không hợp lệ
        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            // Verify token is valid
            string tokenQuery = @"
                SELECT u.UserID FROM Users u
                JOIN PasswordResetTokens t ON u.UserID = t.UserID
                WHERE u.Email = @Email AND t.Token = @Token 
                AND t.IsUsed = 0 AND t.ExpiresAt > GETDATE()";

            var tokenParams = new Dictionary<string, object>
            {
                { "@Email", email },
                { "@Token", token }
            };

            var userId = await _databaseService.ExecuteScalarAsync(tokenQuery, tokenParams);
            if (userId == null)
                return false;

            // Create password hash
            using var hmac = new HMACSHA512();
            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
            byte[] passwordSalt = hmac.Key;

            // Update password
            string updateQuery = @"
                UPDATE Users SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt
                WHERE UserID = @UserID;
                
                UPDATE PasswordResetTokens SET IsUsed = 1
                WHERE UserID = @UserID AND Token = @Token";

            var updateParams = new Dictionary<string, object>
            {
                { "@UserID", userId },
                { "@PasswordHash", passwordHash },
                { "@PasswordSalt", passwordSalt },
                { "@Token", token }
            };

            await _databaseService.ExecuteNonQueryAsync(updateQuery, updateParams);
            return true;
        }

        // 1. Phương thức yêu cầu đặt lại mật khẩu
        // 2. Kiểm tra email và tạo token đặt lại mật khẩu
        // 3. Lưu token vào cơ sở dữ liệu với thời gian hết hạn
        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            // Check if user exists
            string userQuery = "SELECT UserID FROM Users WHERE Email = @Email AND IsActive = 1";
            var parameters = new Dictionary<string, object>
            {
                { "@Email", email }
            };

            var userId = await _databaseService.ExecuteScalarAsync(userQuery, parameters);
            if (userId == null)
                return false;

            // Generate token (6-digit numeric OTP)
            Random random = new Random();
            string token = random.Next(100000, 999999).ToString();

            // Store token in database
            string insertQuery = @"
                INSERT INTO PasswordResetTokens (UserID, Token, ExpiresAt)
                VALUES (@UserID, @Token, DATEADD(HOUR, 1, GETDATE()))";

            var insertParams = new Dictionary<string, object>
            {
                { "@UserID", userId },
                { "@Token", token }
            };

            await _databaseService.ExecuteNonQueryAsync(insertQuery, insertParams);

            // Return the token (it will be sent via email in the ViewModel)
            return true;
        }

        // 1. Phương thức đăng xuất
        // 2. Xóa thông tin người dùng hiện tại
        // 3. Làm sạch bộ nhớ đệm người dùng
        public void Logout()
        {
            // Clear the current user information
            _currentUser = null;
        }
    }
}