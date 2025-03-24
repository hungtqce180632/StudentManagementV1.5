using System;
using System.Collections.Generic;

namespace StudentManagementV1._5.Models
{
    public class Subject
    {
        public int SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? Credits { get; set; }
        public bool IsActive { get; set; }
        
        // Statistics properties (not mapped to database)
        public int ClassCount { get; set; }
        public int TeacherCount { get; set; }
    }
}
