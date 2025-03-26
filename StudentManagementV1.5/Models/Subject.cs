using System;

namespace StudentManagementV1._5.Models
{
    // Lớp Subject
    // + Tại sao cần sử dụng: Lưu trữ thông tin môn học trong hệ thống
    // + Lớp này ánh xạ với bảng Subjects trong cơ sở dữ liệu
    // + Chức năng chính: Quản lý thông tin chi tiết về các môn học
    public class Subject
    {
        // 1. Khóa chính của bảng Subjects
        // 2. Được sử dụng để xác định duy nhất một môn học
        // 3. Tự động tạo khi thêm mới môn học
        public int SubjectID { get; set; }
        
        // 1. Tên môn học
        // 2. Hiển thị cho người dùng
        // 3. Phải là duy nhất trong hệ thống
        public string SubjectName { get; set; } = string.Empty;
        
        // 1. Mô tả về môn học
        // 2. Cung cấp thông tin chi tiết về nội dung môn học
        // 3. Có thể để trống
        public string Description { get; set; } = string.Empty;
        
        // 1. Số tín chỉ của môn học
        // 2. Dùng để tính toán khối lượng học tập
        // 3. Thường là số dương
        public int Credits { get; set; }
        
        // 1. Trạng thái hoạt động của môn học
        // 2. Xác định môn học có đang được giảng dạy hay không
        // 3. True = đang hoạt động, False = đã ngừng giảng dạy
        public bool IsActive { get; set; } = true;
        
        // 1. Số lượng lớp học đang dạy môn này
        // 2. Tính toán từ quan hệ với bảng TeacherSubjects hoặc ClassSchedules
        // 3. Không lưu trong cơ sở dữ liệu, chỉ dùng để hiển thị
        public int ClassCount { get; set; }
        
        // 1. Số lượng giáo viên dạy môn này
        // 2. Tính toán từ quan hệ với bảng TeacherSubjects
        // 3. Không lưu trong cơ sở dữ liệu, chỉ dùng để hiển thị
        public int TeacherCount { get; set; }
    }
}
