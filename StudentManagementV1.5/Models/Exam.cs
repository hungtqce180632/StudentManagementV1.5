using System;

namespace StudentManagementV1._5.Models
{
    public class Exam
    {
        public int ExamID { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty; // For display
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty; // For display 
        public string ExamName { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public int? Duration { get; set; } // In minutes, nullable
        public int TotalMarks { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
