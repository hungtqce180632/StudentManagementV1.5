using System;

namespace StudentManagementV1._5.Models
{
    // Lớp Submission
    // + Tại sao cần sử dụng: Lưu trữ thông tin về bài nộp của học sinh
    // + Lớp này được sử dụng trong các ViewModel liên quan đến chấm điểm và xem bài nộp
    // + Chức năng chính: Chứa thông tin chi tiết về bài nộp, điểm số và nhận xét
    public class Submission
    {
        // ID của bài nộp
        public int SubmissionID { get; set; }
        
        // ID của bài tập mà bài nộp thuộc về
        public int AssignmentID { get; set; }
        
        // Tiêu đề của bài tập (không lưu trong DB, chỉ để hiển thị)
        public string AssignmentTitle { get; set; } = string.Empty;
        
        // ID của học sinh nộp bài
        public int StudentID { get; set; }
        
        // Tên đầy đủ của học sinh (không lưu trong DB, chỉ để hiển thị)
        public string StudentName { get; set; } = string.Empty;
        
        // Nội dung bài nộp của học sinh
        public string Content { get; set; } = string.Empty;
        
        // Đường dẫn đến file đính kèm (nếu có)
        public string AttachmentPath { get; set; } = string.Empty;
        
        // Ngày nộp bài
        public DateTime SubmissionDate { get; set; }
        
        // Trạng thái bài nộp (Submitted, Graded, Rejected)
        public string Status { get; set; } = "Submitted";
        
        // Điểm số được chấm
        public int? Score { get; set; }
        
        // Nhận xét của giáo viên
        public string Feedback { get; set; } = string.Empty;
        
        // Deadline của bài tập (không lưu trong DB, chỉ để hiển thị)
        public DateTime DueDate { get; set; }
        
        // Kiểm tra bài nộp có đúng hạn không
        public bool IsOnTime => SubmissionDate <= DueDate;
    }
}
