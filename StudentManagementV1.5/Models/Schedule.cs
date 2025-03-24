using System;

namespace StudentManagementV1._5.Models
{
    public class Schedule
    {
        public int ScheduleID { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int TeacherID { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; } = string.Empty;
    }

    public class SchoolClass
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
