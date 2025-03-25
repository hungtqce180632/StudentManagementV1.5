using System;

namespace StudentManagementV1._5.Models
{
    // Lớp Notification
    // + Tại sao cần sử dụng: Lưu trữ và quản lý thông báo trong hệ thống
    // + Lớp này liên kết với các đối tượng User, Class hoặc các đối tượng khác thông qua RecipientID
    // + Chức năng chính: Gửi và theo dõi thông báo đến người dùng trong hệ thống
    public class Notification
    {
        // 1. Khóa chính của bảng Notification
        // 2. Đại diện cho một thông báo trong hệ thống
        // 3. Tự động tạo khi thêm mới thông báo
        public int NotificationID { get; set; }
        
        // 1. Tiêu đề của thông báo
        // 2. Hiển thị ngắn gọn nội dung chính của thông báo
        // 3. Được hiển thị trong danh sách thông báo
        public string Title { get; set; } = string.Empty;
        
        // 1. Nội dung chi tiết của thông báo
        // 2. Chứa thông tin đầy đủ mà người gửi muốn truyền đạt
        // 3. Được hiển thị khi người dùng xem chi tiết thông báo
        public string Message { get; set; } = string.Empty;
        
        // 1. ID của người gửi thông báo
        // 2. Thường là UserID của giáo viên hoặc quản trị viên
        // 3. Dùng để truy xuất thông tin người gửi
        public int SenderID { get; set; }
        
        // 1. Tên người gửi thông báo
        // 2. Được lưu trực tiếp để tiện hiển thị mà không cần truy vấn thêm
        // 3. Thường là tên đầy đủ của người gửi
        public string SenderName { get; set; } = string.Empty;
        
        // 1. Loại đối tượng nhận thông báo
        // 2. Xác định thông báo được gửi cho đối tượng nào
        // 3. Có thể là 'All', 'Class', 'Teacher', 'Student'
        public string RecipientType { get; set; } = string.Empty; // 'All', 'Class', 'Teacher', 'Student'
        
        // 1. ID của đối tượng nhận thông báo
        // 2. Có thể là ClassID, TeacherID hoặc StudentID tùy thuộc vào RecipientType
        // 3. Nullable vì có thể thông báo được gửi cho tất cả (RecipientType = 'All')
        public int? RecipientID { get; set; }
        
        // 1. Tên của đối tượng nhận thông báo
        // 2. Được lưu trực tiếp để tiện hiển thị mà không cần truy vấn thêm
        // 3. Có thể là tên lớp, tên giáo viên hoặc tên học sinh
        public string RecipientName { get; set; } = string.Empty;
        
        // 1. Trạng thái đã đọc của thông báo
        // 2. Xác định người nhận đã đọc thông báo hay chưa
        // 3. True = đã đọc, False = chưa đọc
        public bool IsRead { get; set; }
        
        // 1. Thời gian tạo thông báo
        // 2. Được gán tự động khi thông báo được tạo
        // 3. Dùng để sắp xếp và hiển thị thông báo theo thứ tự thời gian
        public DateTime CreatedDate { get; set; }
        
        // 1. Thời gian hết hạn của thông báo
        // 2. Xác định thời điểm thông báo không còn hiệu lực
        // 3. Nullable vì không phải tất cả thông báo đều có thời hạn
        public DateTime? ExpiryDate { get; set; }
        
        // 1. Thuộc tính tính toán xác định thông báo đã hết hạn chưa
        // 2. So sánh ExpiryDate với thời gian hiện tại
        // 3. True = đã hết hạn, False = còn hiệu lực hoặc không có thời hạn
        // Helper properties for UI
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now;
        
        // 1. Thuộc tính phụ trợ để hiển thị trạng thái thông báo
        // 2. Kết hợp thông tin về hết hạn và đã đọc
        // 3. Trả về "Expired", "Read" hoặc "Unread"
        public string StatusDisplay => IsExpired ? "Expired" : (IsRead ? "Read" : "Unread");
    }
}
