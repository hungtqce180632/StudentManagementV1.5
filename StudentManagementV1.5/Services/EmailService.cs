using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace StudentManagementV1._5.Services
{
    public class EmailService
    {
        // You should store these in app config instead of hardcoding
        private readonly string _smtpServer = "smtp.example.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUsername = "your_email@example.com";
        private readonly string _smtpPassword = "your_password";

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