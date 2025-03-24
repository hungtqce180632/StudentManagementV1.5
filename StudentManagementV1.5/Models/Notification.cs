using System;

namespace StudentManagementV1._5.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int SenderID { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string RecipientType { get; set; } = string.Empty; // 'All', 'Class', 'Teacher', 'Student'
        public int? RecipientID { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        
        // Helper properties for UI
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now;
        public string StatusDisplay => IsExpired ? "Expired" : (IsRead ? "Read" : "Unread");
    }
}
