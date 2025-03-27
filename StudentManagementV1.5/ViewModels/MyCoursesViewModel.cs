using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp MyCoursesViewModel
    // + Tại sao cần sử dụng: Quản lý và hiển thị các khóa học mà học sinh đã đăng ký
    // + Lớp này được gọi từ màn hình dashboard của học sinh
    // + Chức năng chính: Hiển thị danh sách khóa học, tìm kiếm và lọc khóa học
    public class MyCoursesViewModel : ViewModelBase
    {
        // Services
        private readonly AuthenticationService _authService;
        private readonly NavigationService _navigationService;
        private readonly DatabaseService _databaseService;

        // Collections
        private ObservableCollection<Course> _courses = new ObservableCollection<Course>();
        private ObservableCollection<Semester> _semesters = new ObservableCollection<Semester>();
        
        // Selected filter items
        private Semester _selectedSemester;
        
        // State tracking
        private bool _isLoading;
        private string _searchText = string.Empty;
        private string _emptyStateMessage = "No courses found.";

        // Properties
        public ObservableCollection<Course> Courses
        {
            get => _courses;
            set => SetProperty(ref _courses, value);
        }

        public ObservableCollection<Semester> Semesters
        {
            get => _semesters;
            set => SetProperty(ref _semesters, value);
        }

        public Semester SelectedSemester
        {
            get => _selectedSemester;
            set => SetProperty(ref _selectedSemester, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string EmptyStateMessage
        {
            get => _emptyStateMessage;
            set => SetProperty(ref _emptyStateMessage, value);
        }

        public bool IsEmpty => Courses.Count == 0;

        // Commands
        public ICommand BackCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand ViewCourseDetailsCommand { get; }
        public ICommand ViewCourseAssignmentsCommand { get; }

        // Constructor
        public MyCoursesViewModel(AuthenticationService authService, NavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            _databaseService = new DatabaseService();

            // Initialize commands
            BackCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.StudentDashboard));
            SearchCommand = new RelayCommand(param => LoadCoursesAsync());
            ApplyFiltersCommand = new RelayCommand(param => LoadCoursesAsync());
            ClearFiltersCommand = new RelayCommand(param => ClearFilters());
            ViewCourseDetailsCommand = new RelayCommand(ViewCourseDetails);
            ViewCourseAssignmentsCommand = new RelayCommand(ViewCourseAssignments);

            // Load data
            LoadSemestersAsync();
            LoadCoursesAsync();
        }

        // Load the list of available semesters
        private async void LoadSemestersAsync()
        {
            try
            {
                // Clear and add default "All Semesters" option first
                Semesters.Clear();
                Semesters.Add(new Semester { SemesterID = 0, SemesterName = "All Semesters" });
                
                try
                {
                    string query = @"
                        SELECT SemesterID, SemesterName 
                        FROM Semesters 
                        ORDER BY StartDate DESC";

                    var result = await _databaseService.ExecuteQueryAsync(query);
                    
                    foreach (DataRow row in result.Rows)
                    {
                        Semesters.Add(new Semester
                        {
                            SemesterID = Convert.ToInt32(row["SemesterID"]),
                            SemesterName = row["SemesterName"].ToString()
                        });
                    }
                }
                catch (Exception)
                {
                    // If Semesters table doesn't exist, add default academic years
                    Semesters.Add(new Semester { SemesterID = 1, SemesterName = "Fall 2024" });
                    Semesters.Add(new Semester { SemesterID = 2, SemesterName = "Spring 2025" });
                }
                
                // Set the default selection to "All Semesters"
                SelectedSemester = Semesters.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading semesters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Load the student's courses based on filters
        private async void LoadCoursesAsync()
        {
            try
            {
                IsLoading = true;
                EmptyStateMessage = "No courses found.";
                
                // Get student ID
                int userId = _authService.CurrentUser?.UserID ?? 0;
                if (userId == 0)
                {
                    EmptyStateMessage = "Student information not found. Please log in again.";
                    OnPropertyChanged(nameof(IsEmpty));
                    return;
                }
                
                // Get the student ID from the user ID
                string studentQuery = "SELECT StudentID FROM Students WHERE UserID = @UserID";
                var studentParams = new Dictionary<string, object> { { "@UserID", userId } };
                var studentResult = await _databaseService.ExecuteScalarAsync(studentQuery, studentParams);
                
                if (studentResult == null)
                {
                    EmptyStateMessage = "Student information not found. Please log in again.";
                    OnPropertyChanged(nameof(IsEmpty));
                    return;
                }
                
                int studentId = Convert.ToInt32(studentResult);
                
                // Build the query to get courses
                string query = @"
                    SELECT 
                        ts.TeacherSubjectID AS CourseID,
                        s.SubjectID,
                        s.SubjectName,
                        c.ClassID,
                        c.ClassName,
                        t.TeacherID,
                        t.FirstName + ' ' + t.LastName AS TeacherName,
                        c.AcademicYear AS SemesterName,
                        0 AS SemesterID,
                        (
                            SELECT TOP 1 
                                'Schedule: ' + cs.DayOfWeek + ' ' + 
                                CONVERT(VARCHAR, cs.StartTime, 108) + ' - ' + 
                                CONVERT(VARCHAR, cs.EndTime, 108) + ', Room: ' + cs.Room
                            FROM ClassSchedules cs
                            WHERE cs.ClassID = c.ClassID AND cs.SubjectID = s.SubjectID
                        ) AS Schedule
                    FROM Students st
                    JOIN Classes c ON st.ClassID = c.ClassID
                    JOIN TeacherSubjects ts ON c.ClassID = ts.ClassID
                    JOIN Subjects s ON ts.SubjectID = s.SubjectID
                    JOIN Teachers t ON ts.TeacherID = t.TeacherID
                    WHERE st.StudentID = @StudentID";
                
                var parameters = new Dictionary<string, object>
                {
                    { "@StudentID", studentId }
                };
                
                // Apply semester filter if selected (using AcademicYear instead of SemesterID)
                if (SelectedSemester != null && SelectedSemester.SemesterID != 0)
                {
                    // For systems without Semesters table, filter by the semester name in AcademicYear
                    query += " AND c.AcademicYear LIKE @SemesterName";
                    parameters.Add("@SemesterName", $"%{SelectedSemester.SemesterName}%");
                }
                
                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query += @" AND (
                        s.SubjectName LIKE @SearchText OR
                        c.ClassName LIKE @SearchText OR
                        t.FirstName + ' ' + t.LastName LIKE @SearchText
                    )";
                    parameters.Add("@SearchText", $"%{SearchText}%");
                }
                
                query += " ORDER BY s.SubjectName";
                
                var result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                Courses.Clear();
                
                foreach (DataRow row in result.Rows)
                {
                    Courses.Add(new Course
                    {
                        CourseID = Convert.ToInt32(row["CourseID"]),
                        SubjectID = Convert.ToInt32(row["SubjectID"]),
                        SubjectName = row["SubjectName"].ToString(),
                        ClassID = Convert.ToInt32(row["ClassID"]),
                        ClassName = row["ClassName"].ToString(),
                        ClassInfo = $"Class: {row["ClassName"]}",
                        TeacherID = Convert.ToInt32(row["TeacherID"]),
                        TeacherName = $"Teacher: {row["TeacherName"]}",
                        Schedule = row["Schedule"] != DBNull.Value ? row["Schedule"].ToString() : "No schedule available",
                        SemesterID = Convert.ToInt32(row["SemesterID"]),
                        SemesterName = row["SemesterName"].ToString()
                    });
                }
                
                if (Courses.Count == 0)
                {
                    if (!string.IsNullOrWhiteSpace(SearchText))
                    {
                        EmptyStateMessage = $"No courses found matching '{SearchText}'.";
                    }
                    else if (SelectedSemester != null && SelectedSemester.SemesterID != 0)
                    {
                        EmptyStateMessage = $"No courses found for {SelectedSemester.SemesterName}.";
                    }
                }
                
                OnPropertyChanged(nameof(IsEmpty));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading courses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Clear all filters and reload
        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedSemester = Semesters.FirstOrDefault();
            LoadCoursesAsync();
        }

        // View course details
        private void ViewCourseDetails(object parameter)
        {
            if (parameter is Course course)
            {
                MessageBox.Show($"Course details for {course.SubjectName} will be implemented in a future update.", 
                    "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // For future implementation:
                // _navigationService.NavigateToWithParameter(AppViews.CourseDetails, course);
            }
        }

        // View course assignments
        private void ViewCourseAssignments(object parameter)
        {
            if (parameter is Course course)
            {
                MessageBox.Show($"Assignments for {course.SubjectName} will be implemented in a future update.", 
                    "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // For future implementation:
                // _navigationService.NavigateToWithParameter(AppViews.ViewAssignments, course);
            }
        }
    }
}
