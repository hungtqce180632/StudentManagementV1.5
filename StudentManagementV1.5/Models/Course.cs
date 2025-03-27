namespace StudentManagementV1._5.Models
{
    // Model class representing a course for a student
    public class Course
    {
        public int CourseID { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string ClassInfo { get; set; } = string.Empty;
        public int TeacherID { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;
        public int SemesterID { get; set; }
    }

    // Model class representing a semester
    public class Semester
    {
        public int SemesterID { get; set; }
        public string SemesterName { get; set; } = string.Empty;
    }
}
