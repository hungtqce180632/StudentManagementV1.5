using System;

namespace StudentManagementV1._5.Models
{
    // Lớp Schedule
    // + Tại sao cần sử dụng: Lưu trữ thông tin lịch học trong hệ thống
    // + Lớp này ánh xạ với bảng ClassSchedules trong cơ sở dữ liệu
    // + Chức năng chính: Quản lý thông tin chi tiết về lịch học các môn
    public class Schedule
    {
        // 1. Khóa chính của bảng ClassSchedules
        // 2. Được sử dụng để xác định duy nhất một lịch học
        // 3. Tự động tạo khi thêm mới lịch học
        public int ScheduleID { get; set; }
        
        // 1. Khóa ngoại tham chiếu đến bảng Classes
        // 2. Xác định lớp học có lịch này
        // 3. Bắt buộc phải có giá trị
        public int ClassID { get; set; }
        
        // 1. Tên lớp học để hiển thị
        // 2. Lấy từ bảng Classes
        // 3. Giúp người dùng dễ nhận biết lớp học
        public string ClassName { get; set; } = string.Empty;
        
        // 1. Khóa ngoại tham chiếu đến bảng Subjects
        // 2. Xác định môn học được dạy trong lịch này
        // 3. Bắt buộc phải có giá trị
        public int SubjectID { get; set; }
        
        // 1. Tên môn học để hiển thị
        // 2. Lấy từ bảng Subjects
        // 3. Giúp người dùng dễ nhận biết môn học
        public string SubjectName { get; set; } = string.Empty;
        
        // 1. Khóa ngoại tham chiếu đến bảng Teachers
        // 2. Xác định giáo viên giảng dạy trong lịch này
        // 3. Bắt buộc phải có giá trị
        public int TeacherID { get; set; }
        
        // 1. Tên giáo viên để hiển thị
        // 2. Lấy từ bảng Teachers (FirstName + LastName)
        // 3. Giúp người dùng dễ nhận biết giáo viên
        public string TeacherName { get; set; } = string.Empty;
        
        // 1. Ngày trong tuần của lịch học
        // 2. Ví dụ: "Monday", "Tuesday", ...
        // 3. Bắt buộc phải có giá trị
        public string DayOfWeek { get; set; } = string.Empty;
        
        // 1. Thời gian bắt đầu của tiết học
        // 2. Định dạng: TimeSpan lưu giờ và phút
        // 3. Bắt buộc phải có giá trị
        public TimeSpan StartTime { get; set; }
        
        // 1. Thời gian kết thúc của tiết học
        // 2. Phải lớn hơn StartTime
        // 3. Bắt buộc phải có giá trị
        public TimeSpan EndTime { get; set; }
        
        // 1. Phòng học diễn ra tiết học
        // 2. Có thể là phòng học thường, phòng thí nghiệm, ...
        // 3. Bắt buộc phải có giá trị
        public string Room { get; set; } = string.Empty;
        
        // 1. Thuộc tính phụ trợ để hiển thị thời gian dạy
        // 2. Kết hợp StartTime và EndTime thành chuỗi dễ đọc
        // 3. Ví dụ: "08:00 - 09:30"
        public string TimeDisplay => $"{StartTime.ToString(@"hh\:mm")} - {EndTime.ToString(@"hh\:mm")}";
        
        // 1. Thuộc tính phụ trợ để hiển thị thông tin đầy đủ
        // 2. Kết hợp tên môn học, giáo viên, thời gian, phòng học
        // 3. Dùng cho hiển thị chi tiết
        public string FullInfo => $"{SubjectName} ({TeacherName}) - {DayOfWeek} {TimeDisplay} - Room: {Room}";
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
