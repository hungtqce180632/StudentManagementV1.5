using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace StudentManagementV1._5.Services
{
    // Lớp EmailService
    // + Tại sao cần sử dụng: Cung cấp chức năng gửi email từ hệ thống
    // + Lớp này được gọi từ các ViewModel khi cần gửi email thông báo hoặc đặt lại mật khẩu
    // + Chức năng chính: Gửi email đặt lại mật khẩu và các thông báo hệ thống
    public class EmailService
    {
        // 1. Địa chỉ máy chủ SMTP để gửi email
        // 2. Nên được lưu trong cấu hình ứng dụng thay vì hard-code
        // 3. Ví dụ: "smtp.gmail.com", "smtp.office365.com", v.v.
        // You should store these in app config instead of hardcoding
        private readonly string _smtpServer = "smtp.example.com";
        
        // 1. Cổng kết nối đến máy chủ SMTP
        // 2. Thông thường là 587 (TLS) hoặc 465 (SSL)
        // 3. Nên được lưu trong cấu hình ứng dụng
        private readonly int _smtpPort = 587;
        
        // 1. Tên đăng nhập vào máy chủ SMTP
        // 2. Thường là địa chỉ email của người gửi
        // 3. Nên được lưu trong cấu hình ứng dụng và mã hóa
        private readonly string _smtpUsername = "your_email@example.com";
        
        // 1. Mật khẩu đăng nhập vào máy chủ SMTP
        // 2. Cần được bảo mật và không nên hard-code
        // 3. Nên được lưu trong cấu hình ứng dụng và mã hóa
        private readonly string _smtpPassword = "your_password";

        // 1. Phương thức gửi email đặt lại mật khẩu
        // 2. Nhận email người nhận và mã token
        // 3. Trong chế độ phát triển, hiển thị token trong MessageBox thay vì gửi email
        public async Task SendPasswordResetEmailAsync(string toEmail, string token)
        {
            try
            {
                // For development/testing: just show the token in a message box instead of sending email
                System.Windows.MessageBox.Show($"Your password reset token is: {token}", 
                    "Password Reset Token", 
                    System.Windows.MessageBoxButton.OK);

                // Uncomment below to actually send emails in production
                /*
                var message = new MailMessage
                {
                    From = new MailAddress(_smtpUsername),
                    Subject = "Password Reset for Student Management System",
                    Body = $"Your password reset token is: {token}\n\nThis token will expire in 1 hour.",
                    IsBodyHtml = false
                };
                message.To.Add(toEmail);

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    await client.SendMailAsync(message);
                }
                */
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Email sending error: {ex.Message}");
                throw;
            }
        }
    }
}