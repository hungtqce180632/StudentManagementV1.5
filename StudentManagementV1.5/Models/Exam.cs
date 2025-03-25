using System;

namespace StudentManagementV1._5.Models
{
    // Lớp Exam
    // + Tại sao cần sử dụng: Lưu trữ thông tin về các kỳ thi và bài kiểm tra
    // + Lớp này liên kết với Subject và Class để xác định đối tượng tham gia và nội dung thi
    // + Chức năng chính: Quản lý thông tin kỳ thi, lịch thi và các thông số liên quan
    public class Exam
    {
        // 1. Khóa chính của bảng Exam
        // 2. Đại diện cho một kỳ thi hoặc bài kiểm tra
        // 3. Tự động tạo khi thêm mới kỳ thi
        public int ExamID { get; set; }
        
        // 1. Khóa ngoại liên kết với bảng Subject
        // 2. Xác định môn học được kiểm tra trong kỳ thi
        // 3. Mỗi kỳ thi thuộc về một môn học cụ thể
        public int SubjectID { get; set; }
        
        // 1. Tên môn học của kỳ thi
        // 2. Được lưu trực tiếp để tiện hiển thị mà không cần truy vấn thêm
        // 3. Thường được lấy từ bảng Subject khi tạo kỳ thi
        public string SubjectName { get; set; } = string.Empty; // For display
        
        // 1. Khóa ngoại liên kết với bảng Class
        // 2. Xác định lớp học tham gia kỳ thi
        // 3. Mỗi kỳ thi được tổ chức cho một lớp cụ thể
        public int ClassID { get; set; }
        
        // 1. Tên lớp tham gia kỳ thi
        // 2. Được lưu trực tiếp để tiện hiển thị mà không cần truy vấn thêm
        // 3. Thường được lấy từ bảng Class khi tạo kỳ thi
        public string ClassName { get; set; } = string.Empty; // For display 
        
        // 1. Tên của kỳ thi hoặc bài kiểm tra
        // 2. Mô tả ngắn gọn loại kỳ thi
        // 3. Ví dụ: "Kiểm tra giữa kỳ", "Thi cuối kỳ", v.v.
        public string ExamName { get; set; } = string.Empty;
        
        // 1. Ngày tổ chức kỳ thi
        // 2. Xác định thời gian diễn ra kỳ thi
        // 3. Quan trọng cho việc lập lịch và thông báo
        public DateTime ExamDate { get; set; }
        
        // 1. Thời gian làm bài (tính bằng phút)
        // 2. Xác định thời lượng của kỳ thi
        // 3. Có thể null nếu không có giới hạn thời gian
        public int? Duration { get; set; } // In minutes, nullable
        
        // 1. Tổng điểm của bài thi
        // 2. Xác định thang điểm cho kỳ thi
        // 3. Thông thường là 10, 100 hoặc các giá trị khác tùy theo quy định
        public int TotalMarks { get; set; }
        
        // 1. Mô tả chi tiết về kỳ thi
        // 2. Cung cấp thông tin bổ sung về nội dung, hình thức thi
        // 3. Giúp học sinh và giáo viên hiểu rõ về kỳ thi
        public string Description { get; set; } = string.Empty;
    }
}
