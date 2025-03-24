using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementV1._5.Models
{
    public class Student
    {
        public int StudentID { get; set; }
        public int UserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int ClassID { get; set; }
        public DateTime EnrollmentDate { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Class? Class { get; set; }
    }
}