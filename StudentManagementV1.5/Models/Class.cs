using System;
using System.Collections.Generic;

namespace StudentManagementV1._5.Models
{
    public class Class
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public int? TeacherID { get; set; }
        public string? TeacherName { get; set; } // Add this property
        public string ClassRoom { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
        public int CurrentStudentCount { get; set; }
        public string AcademicYear { get; set; } = string.Empty; // Changed to string type
        public bool IsActive { get; set; }

        // Navigation properties
        public Teacher? Teacher { get; set; }
        public ICollection<Student>? Students { get; set; }
    }
}
