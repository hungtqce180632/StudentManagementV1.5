using System;

namespace StudentManagementV1._5.Models
{
    // Lớp Assignment
    // + Tại sao cần sử dụng: Lưu trữ thông tin về bài tập được giao
    // + Lớp này được sử dụng trong các ViewModel liên quan đến quản lý bài tập
    // + Chức năng chính: Chứa thông tin chi tiết về bài tập như tiêu đề, mô tả, deadline, v.v.
    public class Assignment
    {
        // ID của bài tập
        public int AssignmentID { get; set; }
        
        // Tiêu đề của bài tập
        public string Title { get; set; } = string.Empty;
        
        // Mô tả chi tiết về bài tập
        public string Description { get; set; } = string.Empty;
        
        // Ngày tạo bài tập
        public DateTime CreatedDate { get; set; }
        
        // Deadline nộp bài
        public DateTime DueDate { get; set; }
        
        // Điểm tối đa cho bài tập
        public int MaxPoints { get; set; }
        
        // ID của giáo viên tạo bài tập
        public int TeacherID { get; set; }
        
        // Tên của giáo viên (không lưu trong DB, chỉ để hiển thị)
        public string TeacherName { get; set; } = string.Empty;
        
        // ID của môn học mà bài tập thuộc về
        public int SubjectID { get; set; }
        
        // Tên của môn học (không lưu trong DB, chỉ để hiển thị)
        public string SubjectName { get; set; } = string.Empty;
        
        // ID của lớp học được giao bài tập
        public int ClassID { get; set; }
        
        // Tên của lớp học (không lưu trong DB, chỉ để hiển thị)
        public string ClassName { get; set; } = string.Empty;
        
        // Số lượng bài đã nộp (không lưu trong DB, chỉ để hiển thị)
        public int SubmissionCount { get; set; }

        // Trạng thái hiện tại của bài tập (Draft, Published, Closed)
        public string Status { get; set; } = "Draft";

        // Trạng thái deadline: Quá hạn, Sắp hết hạn, Còn nhiều thời gian
        public string GetDueStatus()
        {
            var timeLeft = DueDate - DateTime.Now;
            
            if (timeLeft.TotalHours < 0)
                return "Overdue";
            if (timeLeft.TotalHours < 24)
                return "Due Soon";
            return "Upcoming";
        }
    }
}
