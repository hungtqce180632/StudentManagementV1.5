using System;
using System.Collections.Generic;

namespace StudentManagementV1._5.Models
{
    // Lớp Class
    // + Tại sao cần sử dụng: Lưu trữ thông tin về các lớp học trong trường
    // + Lớp này liên kết với Teacher (giáo viên chủ nhiệm) và có nhiều Student (học sinh trong lớp)
    // + Chức năng chính: Quản lý thông tin lớp học, phân công giáo viên và theo dõi số lượng học sinh
    public class Class
    {
        // 1. Khóa chính của bảng Class
        // 2. Đại diện cho một lớp học trong trường
        // 3. Được sử dụng để liên kết với các bảng khác
        public int ClassID { get; set; }
        
        // 1. Tên lớp học
        // 2. Dùng để hiển thị và xác định lớp
        // 3. Thường là duy nhất trong hệ thống
        public string ClassName { get; set; } = string.Empty;
        
        // 1. Khối lớp hoặc cấp độ của lớp học
        // 2. Ví dụ: "Lớp 1", "Lớp 10", "Năm thứ nhất", v.v.
        // 3. Dùng để phân loại lớp theo cấp học
        public string Grade { get; set; } = string.Empty;
        
        // 1. Khóa ngoại liên kết với bảng Teacher
        // 2. Xác định giáo viên chủ nhiệm của lớp
        // 3. Có thể null nếu lớp chưa được phân công giáo viên chủ nhiệm
        public int? TeacherID { get; set; }
        
        // 1. Tên của giáo viên chủ nhiệm
        // 2. Được lưu trực tiếp để tiện hiển thị mà không cần truy vấn thêm
        // 3. Có thể null nếu chưa có giáo viên chủ nhiệm
        public string? TeacherName { get; set; } // Add this property
        
        // 1. Phòng học chính của lớp
        // 2. Xác định nơi lớp thường xuyên học tập
        // 3. Có thể là số phòng hoặc tên phòng cụ thể
        public string ClassRoom { get; set; } = string.Empty;
        
        // 1. Sức chứa tối đa của lớp học
        // 2. Xác định số lượng học sinh tối đa có thể có trong lớp
        // 3. Dùng để quản lý quy mô lớp và kiểm soát việc thêm học sinh
        public int MaxCapacity { get; set; }
        
        // 1. Số lượng học sinh hiện có trong lớp
        // 2. Được cập nhật khi thêm hoặc xóa học sinh
        // 3. Dùng để kiểm tra lớp đã đầy hay chưa
        public int CurrentStudentCount { get; set; }
        
        // 1. Năm học của lớp
        // 2. Ví dụ: "2023-2024"
        // 3. Dùng để phân loại và quản lý lớp theo năm học
        public string AcademicYear { get; set; } = string.Empty; // Changed to string type
        
        // 1. Trạng thái hoạt động của lớp
        // 2. Xác định lớp có đang hoạt động hay không
        // 3. True = đang hoạt động, False = đã kết thúc hoặc tạm dừng
        public bool IsActive { get; set; }

        // 1. Thuộc tính điều hướng đến đối tượng Teacher
        // 2. Được thiết lập khi truy vấn dữ liệu có Include
        // 3. Cho phép truy cập thông tin Teacher từ Class
        // Navigation properties
        public Teacher? Teacher { get; set; }
        
        // 1. Thuộc tính điều hướng đến danh sách học sinh trong lớp
        // 2. Được thiết lập khi truy vấn dữ liệu có Include
        // 3. Cho phép truy cập danh sách Student từ Class
        public ICollection<Student>? Students { get; set; }
    }
}
