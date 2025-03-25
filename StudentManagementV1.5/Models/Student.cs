using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementV1._5.Models
{
    // Lớp Student
    // + Tại sao cần sử dụng: Lưu trữ thông tin chi tiết về học sinh trong hệ thống
    // + Lớp này liên kết với User thông qua UserID và liên kết với Class thông qua ClassID
    // + Chức năng chính: Quản lý thông tin cá nhân và học tập của học sinh
    public class Student
    {
        // 1. Khóa chính của bảng Student
        // 2. Được sử dụng để liên kết với các bảng khác
        // 3. Tự động tạo khi thêm mới học sinh
        public int StudentID { get; set; }
        
        // 1. Khóa ngoại liên kết với bảng User
        // 2. Mỗi học sinh được liên kết với một tài khoản User
        // 3. Dùng để truy xuất thông tin đăng nhập của học sinh
        public int UserID { get; set; }
        
        // 1. Tên của học sinh
        // 2. Là một phần của tên đầy đủ
        // 3. Được hiển thị trong giao diện người dùng
        public string FirstName { get; set; } = string.Empty;
        
        // 1. Họ của học sinh
        // 2. Là một phần của tên đầy đủ
        // 3. Được hiển thị trong giao diện người dùng
        public string LastName { get; set; } = string.Empty;
        
        // 1. Ngày sinh của học sinh
        // 2. Dùng để tính tuổi và quản lý thông tin cá nhân
        // 3. Có thể null nếu thông tin chưa được cung cấp
        public DateTime? DateOfBirth { get; set; }
        
        // 1. Giới tính của học sinh
        // 2. Thông tin cá nhân cơ bản
        // 3. Thường là "Nam" hoặc "Nữ" hoặc các giá trị khác
        public string Gender { get; set; } = string.Empty;
        
        // 1. Địa chỉ của học sinh
        // 2. Thông tin liên hệ cho mục đích hành chính
        // 3. Có thể là địa chỉ nhà hoặc nơi ở hiện tại
        public string Address { get; set; } = string.Empty;
        
        // 1. Số điện thoại liên hệ của học sinh hoặc phụ huynh
        // 2. Dùng để liên lạc trong trường hợp cần thiết
        // 3. Thường được định dạng theo chuẩn của quốc gia
        public string PhoneNumber { get; set; } = string.Empty;
        
        // 1. Khóa ngoại liên kết với bảng Class
        // 2. Xác định học sinh thuộc lớp nào
        // 3. Mỗi học sinh chỉ thuộc một lớp tại một thời điểm
        public int ClassID { get; set; }
        
        // 1. Ngày học sinh nhập học
        // 2. Dùng để tính thời gian học tập và quản lý khóa học
        // 3. Thông tin quan trọng cho quản lý học sinh
        public DateTime EnrollmentDate { get; set; }

        // 1. Thuộc tính điều hướng đến đối tượng User
        // 2. Được thiết lập khi truy vấn dữ liệu có Include
        // 3. Cho phép truy cập thông tin User từ Student
        // Navigation properties
        public User? User { get; set; }
        
        // 1. Thuộc tính điều hướng đến đối tượng Class
        // 2. Được thiết lập khi truy vấn dữ liệu có Include
        // 3. Cho phép truy cập thông tin Class từ Student
        public Class? Class { get; set; }
    }
}