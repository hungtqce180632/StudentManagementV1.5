using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementV1._5.Models
{
    // Lớp Teacher
    // + Tại sao cần sử dụng: Lưu trữ thông tin chi tiết về giáo viên trong hệ thống
    // + Lớp này liên kết với User thông qua UserID và liên kết với Class như một giáo viên chủ nhiệm
    // + Chức năng chính: Quản lý thông tin cá nhân và chuyên môn của giáo viên
    public class Teacher
    {
        // 1. Khóa chính của bảng Teacher
        // 2. Được sử dụng để liên kết với các bảng khác
        // 3. Tự động tạo khi thêm mới giáo viên
        public int TeacherID { get; set; }
        
        // 1. Khóa ngoại liên kết với bảng User
        // 2. Mỗi giáo viên được liên kết với một tài khoản User
        // 3. Dùng để truy xuất thông tin đăng nhập của giáo viên
        public int UserID { get; set; }
        
        // 1. Tên của giáo viên
        // 2. Là một phần của tên đầy đủ
        // 3. Được hiển thị trong giao diện người dùng
        public string FirstName { get; set; } = string.Empty;
        
        // 1. Họ của giáo viên
        // 2. Là một phần của tên đầy đủ
        // 3. Được hiển thị trong giao diện người dùng
        public string LastName { get; set; } = string.Empty;
        
        // 1. Ngày sinh của giáo viên
        // 2. Dùng để tính tuổi và quản lý thông tin cá nhân
        // 3. Có thể null nếu thông tin chưa được cung cấp
        public DateTime? DateOfBirth { get; set; }
        
        // 1. Giới tính của giáo viên
        // 2. Thông tin cá nhân cơ bản
        // 3. Thường là "Nam" hoặc "Nữ" hoặc các giá trị khác
        public string Gender { get; set; } = string.Empty;
        
        // 1. Địa chỉ của giáo viên
        // 2. Thông tin liên hệ cho mục đích hành chính
        // 3. Có thể là địa chỉ nhà hoặc nơi ở hiện tại
        public string Address { get; set; } = string.Empty;
        
        // 1. Số điện thoại liên hệ của giáo viên
        // 2. Dùng để liên lạc trong trường hợp cần thiết
        // 3. Thường được định dạng theo chuẩn của quốc gia
        public string PhoneNumber { get; set; } = string.Empty;
        
        // 1. Ngày giáo viên được tuyển dụng
        // 2. Dùng để tính thâm niên công tác
        // 3. Thông tin quan trọng cho quản lý nhân sự
        public DateTime HireDate { get; set; }
        
        // 1. Chuyên môn hoặc lĩnh vực giảng dạy của giáo viên
        // 2. Xác định giáo viên có thể dạy môn học nào
        // 3. Ví dụ: "Toán", "Văn", "Tiếng Anh", v.v.
        public string Specialization { get; set; } = string.Empty;

        // 1. Thuộc tính điều hướng đến đối tượng User
        // 2. Được thiết lập khi truy vấn dữ liệu có Include
        // 3. Cho phép truy cập thông tin User từ Teacher
        // Navigation property
        public User? User { get; set; }
    }
}