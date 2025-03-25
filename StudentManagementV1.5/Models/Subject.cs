using System;
using System.Collections.Generic;

namespace StudentManagementV1._5.Models
{
    // Lớp Subject
    // + Tại sao cần sử dụng: Lưu trữ thông tin về các môn học trong hệ thống
    // + Lớp này liên kết với Schedule, Exam và có thể liên kết với Teacher qua các bảng trung gian
    // + Chức năng chính: Quản lý danh sách môn học và thông tin liên quan
    public class Subject
    {
        // 1. Khóa chính của bảng Subject
        // 2. Được sử dụng để liên kết với các bảng khác
        // 3. Tự động tạo khi thêm mới môn học
        public int SubjectID { get; set; }
        
        // 1. Tên của môn học
        // 2. Dùng để hiển thị và xác định môn học
        // 3. Thường là duy nhất trong hệ thống
        public string SubjectName { get; set; } = string.Empty;
        
        // 1. Mô tả chi tiết về môn học
        // 2. Cung cấp thông tin tổng quan về nội dung môn học
        // 3. Giúp giáo viên và học sinh hiểu rõ về môn học
        public string Description { get; set; } = string.Empty;
        
        // 1. Số tín chỉ của môn học
        // 2. Xác định trọng số và thời lượng của môn học
        // 3. Có thể null nếu không áp dụng hệ thống tín chỉ
        public int? Credits { get; set; }
        
        // 1. Trạng thái hoạt động của môn học
        // 2. Xác định môn học có được mở lớp và giảng dạy không
        // 3. True = đang hoạt động, False = đã ngừng giảng dạy
        public bool IsActive { get; set; }
        
        // 1. Số lượng lớp học đang dạy môn học này
        // 2. Thuộc tính thống kê, không lưu trong database
        // 3. Được tính toán khi truy vấn dữ liệu
        // Statistics properties (not mapped to database)
        public int ClassCount { get; set; }
        
        // 1. Số lượng giáo viên dạy môn học này
        // 2. Thuộc tính thống kê, không lưu trong database
        // 3. Được tính toán khi truy vấn dữ liệu
        public int TeacherCount { get; set; }
    }
}
