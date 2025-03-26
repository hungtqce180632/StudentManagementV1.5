using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementV1._5.Models
{
    // Lớp User
    // + Tại sao cần sử dụng: Lưu trữ thông tin tài khoản người dùng trong hệ thống
    // + Lớp này liên kết với Teacher, Student thông qua UserID và là điểm truy cập chính cho đăng nhập
    // + Chức năng chính: Quản lý thông tin đăng nhập và phân quyền người dùng
    public class User
    {
        // 1. Khóa chính của bảng User
        // 2. Được sử dụng để liên kết với các bảng khác
        // 3. Tự động tạo khi thêm mới người dùng
        public int UserID { get; set; }
        
        // 1. Tên đăng nhập của người dùng
        // 2. Được sử dụng khi đăng nhập vào hệ thống
        // 3. Phải là duy nhất trong hệ thống
        public string Username { get; set; } = string.Empty;
        
        // 1. Địa chỉ email của người dùng
        // 2. Dùng để liên hệ và có thể dùng để khôi phục mật khẩu
        // 3. Thường được yêu cầu là duy nhất trong hệ thống
        public string Email { get; set; } = string.Empty;
        
        // 1. Vai trò của người dùng trong hệ thống
        // 2. Xác định quyền hạn và chức năng có thể truy cập
        // 3. Ví dụ: "Admin", "Teacher", "Student"
        public string Role { get; set; } = string.Empty;
        
        // 1. Trạng thái hoạt động của tài khoản
        // 2. Xác định tài khoản có thể đăng nhập hay không
        // 3. True = đang hoạt động, False = đã bị vô hiệu hóa
        public bool IsActive { get; set; }
        
        // 1. Ngày tạo tài khoản
        // 2. Tự động gán khi tạo tài khoản mới
        // 3. Dùng để theo dõi lịch sử tài khoản
        public DateTime CreatedDate { get; set; }
        
        // 1. Thời gian đăng nhập gần nhất
        // 2. Cập nhật mỗi khi người dùng đăng nhập thành công
        // 3. Nullable vì người dùng có thể chưa từng đăng nhập
        public DateTime? LastLoginDate { get; set; }
        
        // 1. Tên đầy đủ của người dùng để hiển thị
        // 2. Được thêm để tiện hiển thị trong giao diện
        // 3. Không cần truy vấn đến bảng Student hoặc Teacher
        // Added property for display name
        public string FullName { get; set; } = string.Empty;
        
        // 1. Thuộc tính phụ trợ để hiển thị trạng thái
        // 2. Chuyển đổi giá trị boolean thành chuỗi dễ đọc
        // 3. Tính toán dựa trên giá trị IsActive
        // Helper property for UI
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        
        // 1. Thuộc tính phụ trợ để hiển thị thời gian đăng nhập
        // 2. Định dạng thời gian đăng nhập thành chuỗi dễ đọc
        // 3. Hiển thị "Never" nếu chưa từng đăng nhập
        // Helper property for last login display
        public string LastLoginDisplay => LastLoginDate.HasValue ? 
            LastLoginDate.Value.ToString("yyyy-MM-dd HH:mm") : 
            "Never";

        // 1. Mảng byte lưu trữ hàm băm mật khẩu
        // 2. Dùng để xác thực khi đăng nhập
        // 3. Không bao giờ lưu trữ mật khẩu thô
        public byte[] PasswordHash { get; set; }
        
        // 1. Mảng byte lưu trữ salt cho mật khẩu
        // 2. Dùng để tăng cường bảo mật cho hàm băm mật khẩu
        // 3. Được tạo ngẫu nhiên khi đặt mật khẩu
        public byte[] PasswordSalt { get; set; }
        
        // 1. Thuộc tính tạm thời để lưu mật khẩu khi tạo mới hoặc đổi mật khẩu
        // 2. Không được lưu vào cơ sở dữ liệu
        // 3. Chỉ dùng trong quá trình tạo hoặc cập nhật
        public string Password { get; set; } = string.Empty;
        
        // 1. Thuộc tính tạm thời để xác nhận mật khẩu
        // 2. Dùng để kiểm tra khớp với Password khi đăng ký hoặc đổi mật khẩu
        // 3. Không được lưu vào cơ sở dữ liệu
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}