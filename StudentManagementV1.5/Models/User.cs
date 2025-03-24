using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementV1._5.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        
        // Added property for display name
        public string FullName { get; set; } = string.Empty;
        
        // Helper property for UI
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        
        // Helper property for last login display
        public string LastLoginDisplay => LastLoginDate.HasValue ? 
            LastLoginDate.Value.ToString("yyyy-MM-dd HH:mm") : 
            "Never";
    }
}