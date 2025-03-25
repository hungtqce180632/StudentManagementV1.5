using System;

namespace StudentManagementV1._5.Models
{
    // Lớp Schedule
    // + Tại sao cần sử dụng: Lưu trữ thông tin lịch học và lịch giảng dạy
    // + Lớp này liên kết với Class, Subject và Teacher để tạo thành thời khóa biểu
    // + Chức năng chính: Quản lý thời gian, địa điểm và người tham gia các buổi học
    public class Schedule
    {
        // 1. Khóa chính của bảng Schedule
        // 2. Đại diện cho một mục trong thời khóa biểu
        // 3. Tự động tạo khi thêm mới lịch học
        public int ScheduleID { get; set; }
        
        // 1. Khóa ngoại liên kết với bảng Class
        // 2. Xác định lớp học tham gia buổi học này
        // 3. Mỗi lịch học thuộc về một lớp cụ thể
        public int ClassID { get; set; }
        
        // 1. Tên lớp học tham gia buổi học
        // 2. Được lưu trực tiếp để tiện hiển thị mà không cần truy vấn thêm
        // 3. Thường được lấy từ bảng Class khi tạo lịch
        public string ClassName { get; set; } = string.Empty;
        
        // 1. Khóa ngoại liên kết với bảng Subject
        // 2. Xác định môn học được giảng dạy trong buổi học
        // 3. Mỗi lịch học dành cho một môn học cụ thể
        public int SubjectID { get; set; }
        
        // 1. Tên môn học được giảng dạy
        // 2. Được lưu trực tiếp để tiện hiển thị mà không cần truy vấn thêm
        // 3. Thường được lấy từ bảng Subject khi tạo lịch
        public string SubjectName { get; set; } = string.Empty;
        
        // 1. Khóa ngoại liên kết với bảng Teacher
        // 2. Xác định giáo viên giảng dạy buổi học
        // 3. Mỗi lịch học có một giáo viên phụ trách
        public int TeacherID { get; set; }
        
        // 1. Tên giáo viên giảng dạy
        // 2. Được lưu trực tiếp để tiện hiển thị mà không cần truy vấn thêm
        // 3. Thường được lấy từ bảng Teacher khi tạo lịch
        public string TeacherName { get; set; } = string.Empty;
        
        // 1. Thứ trong tuần của buổi học
        // 2. Ví dụ: "Thứ Hai", "Thứ Ba", v.v.
        // 3. Là một phần của thông tin thời gian học
        public string DayOfWeek { get; set; } = string.Empty;
        
        // 1. Thời gian bắt đầu buổi học
        // 2. Lưu dưới dạng TimeSpan (giờ:phút:giây)
        // 3. Kết hợp với DayOfWeek để xác định thời điểm học
        public TimeSpan StartTime { get; set; }
        
        // 1. Thời gian kết thúc buổi học
        // 2. Lưu dưới dạng TimeSpan (giờ:phút:giây)
        // 3. Được sử dụng để tính thời lượng và kiểm tra trùng lịch
        public TimeSpan EndTime { get; set; }
        
        // 1. Phòng học hoặc địa điểm diễn ra buổi học
        // 2. Có thể là số phòng, tên phòng hoặc địa điểm cụ thể
        // 3. Giúp học sinh và giáo viên biết nơi diễn ra buổi học
        public string Room { get; set; } = string.Empty;
    }

    // Lớp SchoolClass
    // + Tại sao cần sử dụng: Lưu trữ thông tin về các lớp học trong trường
    // + Lớp này có thể liên kết với Schedule, Student và có thể có các thông tin học thuật
    // + Chức năng chính: Quản lý thông tin cơ bản về lớp học
    public class SchoolClass
    {
        // 1. Khóa chính của bảng SchoolClass
        // 2. Đại diện cho một lớp học trong trường
        // 3. Được sử dụng để liên kết với các bảng khác
        public int ClassID { get; set; }
        
        // 1. Tên lớp học
        // 2. Dùng để hiển thị và xác định lớp
        // 3. Thường là duy nhất trong hệ thống
        public string ClassName { get; set; } = string.Empty;
        
        // 1. Năm học của lớp
        // 2. Ví dụ: "2023-2024"
        // 3. Dùng để phân loại và quản lý lớp theo năm học
        public string AcademicYear { get; set; } = string.Empty;
        
        // 1. Trạng thái hoạt động của lớp
        // 2. Xác định lớp có đang hoạt động hay không
        // 3. True = đang hoạt động, False = đã kết thúc hoặc tạm dừng
        public bool IsActive { get; set; }
    }
}
