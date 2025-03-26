using System;

namespace StudentManagementV1._5.Models
{
    // Lớp Notification
    // + Tại sao cần sử dụng: Lưu trữ thông tin thông báo trong hệ thống
    // + Lớp này ánh xạ với bảng Notifications trong cơ sở dữ liệu
    // + Chức năng chính: Quản lý thông tin chi tiết về các thông báo gửi cho người dùng
    public class Notification
    {
        // 1. Khóa chính của bảng Notifications
        // 2. Được sử dụng để xác định duy nhất một thông báo
        // 3. Tự động tạo khi thêm mới thông báo
        public int NotificationID { get; set; }
        
        // 1. Tiêu đề của thông báo
        // 2. Hiển thị cho người dùng trong danh sách thông báo
        // 3. Nên ngắn gọn, súc tích và mô tả được nội dung
        public string Title { get; set; } = string.Empty;
        
        // 1. Nội dung chi tiết của thông báo
        // 2. Chứa thông tin đầy đủ người dùng cần biết
        // 3. Có thể dài và chi tiết
        public string Message { get; set; } = string.Empty;
        
        // 1. ID của người gửi thông báo
        // 2. Liên kết với bảng Users
        // 3. Để theo dõi người đã tạo thông báo
        public int? SenderID { get; set; }
        
        // 1. Tên người gửi để hiển thị
        // 2. Lấy từ thông tin người dùng có SenderID
        // 3. Giúp người nhận biết ai đã gửi thông báo
        public string SenderName { get; set; } = string.Empty;
        
        // 1. Loại đối tượng nhận thông báo
        // 2. Có thể là "All", "Student", "Teacher", "Admin", "Class", v.v.
        // 3. Xác định nhóm người nhận thông báo
        public string RecipientType { get; set; } = string.Empty;
        
        // 1. ID của đối tượng nhận thông báo
        // 2. Tùy thuộc vào RecipientType, có thể là UserID, ClassID, v.v.
        // 3. Null nếu gửi cho toàn bộ đối tượng trong RecipientType
        public int? RecipientID { get; set; }
        
        // 1. Tên của đối tượng nhận thông báo
        // 2. Hiển thị tên lớp, tên người dùng hoặc nhóm nhận
        // 3. Giúp hiển thị thông tin người nhận trong giao diện
        public string RecipientName { get; set; } = string.Empty;
        
        // 1. Trạng thái đã đọc của thông báo
        // 2. true = đã đọc, false = chưa đọc
        // 3. Dùng để đánh dấu và lọc thông báo
        public bool IsRead { get; set; }
        
        // 1. Ngày tạo thông báo
        // 2. Tự động gán khi tạo thông báo mới
        // 3. Dùng để sắp xếp và lọc thông báo theo thời gian
        public DateTime CreatedDate { get; set; }
        
        // 1. Ngày hết hạn của thông báo
        // 2. Sau thời điểm này, thông báo không còn hiển thị cho người dùng
        // 3. Null nếu thông báo không có thời hạn
        public DateTime? ExpiryDate { get; set; }
        
        // 1. Trạng thái hoạt động của thông báo
        // 2. Một thông báo không hoạt động khi đã hết hạn hoặc bị hủy
        // 3. Dùng để lọc thông báo còn hiệu lực
        public bool IsActive => !ExpiryDate.HasValue || ExpiryDate.Value >= DateTime.Now;
        
        // 1. Thuộc tính phụ hiển thị trạng thái thông báo dưới dạng chuỗi
        // 2. Dựa trên giá trị IsActive và IsRead
        // 3. Dùng cho việc hiển thị trong giao diện
        public string StatusDisplay => !IsActive ? "Expired" : (IsRead ? "Read" : "Unread");
        
        // 1. Thuộc tính phụ định dạng thời gian tạo
        // 2. Chuyển đổi CreatedDate sang định dạng dễ đọc
        // 3. Dùng cho việc hiển thị trong giao diện
        public string CreatedDateDisplay => CreatedDate.ToString("yyyy-MM-dd HH:mm");
        
        // 1. Thuộc tính phụ định dạng thời gian hết hạn
        // 2. Chuyển đổi ExpiryDate sang định dạng dễ đọc hoặc "No Expiry"
        // 3. Dùng cho việc hiển thị trong giao diện
        public string ExpiryDateDisplay => ExpiryDate.HasValue ? 
            ExpiryDate.Value.ToString("yyyy-MM-dd HH:mm") : "No Expiry";
    }
}
