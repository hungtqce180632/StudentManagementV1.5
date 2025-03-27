using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp MySubjectsViewModel
    // + Tại sao cần sử dụng: Quản lý và hiển thị danh sách môn học của giáo viên
    // + Lớp này được gọi từ màn hình My Subjects của giáo viên
    // + Chức năng chính: Hiển thị các môn học mà giáo viên đang giảng dạy
    public class MySubjectsViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly NavigationService _navigationService;
        private readonly AuthenticationService _authService;
        
        private ObservableCollection<SubjectTeachingInfo> _teacherSubjects = new ObservableCollection<SubjectTeachingInfo>();
        private SubjectTeachingInfo _selectedSubject;
        private bool _isLoading;
        private string _errorMessage = string.Empty;

        // Properties
        public ObservableCollection<SubjectTeachingInfo> TeacherSubjects
        {
            get => _teacherSubjects;
            set => SetProperty(ref _teacherSubjects, value);
        }

        public SubjectTeachingInfo SelectedSubject
        {
            get => _selectedSubject;
            set => SetProperty(ref _selectedSubject, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // Commands
        public ICommand BackCommand { get; }
        public ICommand ViewStudentsCommand { get; }
        public ICommand ViewScheduleCommand { get; }

        // Constructor
        public MySubjectsViewModel(DatabaseService databaseService, NavigationService navigationService, AuthenticationService authService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _authService = authService;

            // Initialize commands
            BackCommand = new RelayCommand(_ => _navigationService.NavigateTo(AppViews.TeacherDashboard));
            ViewStudentsCommand = new RelayCommand(param => ViewStudents(param as SubjectTeachingInfo), param => param != null);
            ViewScheduleCommand = new RelayCommand(param => ViewSchedule(param as SubjectTeachingInfo), param => param != null);

            // Load data
            LoadTeacherSubjectsAsync();
        }

        // Load teacher's subjects
        private async void LoadTeacherSubjectsAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                TeacherSubjects.Clear();

                // Get current teacher ID
                int teacherId = _authService.CurrentUser?.UserID ?? 0; // Changed from UserId to UserID
                if (teacherId == 0)
                {
                    ErrorMessage = "Teacher information not found. Please log in again.";
                    return;
                }

                // Query to get subjects taught by this teacher
                string query = @"
                SELECT 
                    ts.TeacherSubjectID,
                    s.SubjectID, 
                    s.SubjectName, 
                    s.Description,
                    s.Credits,
                    c.ClassID,
                    c.ClassName,
                    ts.AcademicYear,
                    (SELECT COUNT(*) FROM Students WHERE ClassID = c.ClassID) AS StudentCount
                FROM TeacherSubjects ts
                JOIN Subjects s ON ts.SubjectID = s.SubjectID
                JOIN Classes c ON ts.ClassID = c.ClassID
                WHERE ts.TeacherID = @TeacherID
                AND s.IsActive = 1
                ORDER BY s.SubjectName, c.ClassName";

                var parameters = new Dictionary<string, object>
                {
                    { "@TeacherID", teacherId }
                };

                var result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                foreach (DataRow row in result.Rows)
                {
                    TeacherSubjects.Add(new SubjectTeachingInfo
                    {
                        TeacherSubjectID = Convert.ToInt32(row["TeacherSubjectID"]),
                        SubjectID = Convert.ToInt32(row["SubjectID"]),
                        SubjectName = row["SubjectName"].ToString() ?? string.Empty,
                        Description = row["Description"].ToString() ?? string.Empty,
                        Credits = Convert.ToInt32(row["Credits"]),
                        ClassID = Convert.ToInt32(row["ClassID"]),
                        ClassName = row["ClassName"].ToString() ?? string.Empty,
                        AcademicYear = row["AcademicYear"].ToString() ?? string.Empty,
                        StudentCount = Convert.ToInt32(row["StudentCount"]) 
                    });
                }

                if (TeacherSubjects.Count == 0)
                {
                    ErrorMessage = "You are not currently assigned to teach any subjects.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading subjects: {ex.Message}";
                MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // View students in a class for this subject
        private void ViewStudents(SubjectTeachingInfo subject)
        {
            if (subject == null) return;
            
            MessageBox.Show($"Em xin loi chuc nang {subject.SubjectName} in {subject.ClassName} Chua duoc lam xong.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // For future implementation:
            // _navigationService.NavigateToWithParameter(AppViews.ClassStudents, subject);
        }

        // View schedule for this subject
        private void ViewSchedule(SubjectTeachingInfo subject)
        {
            if (subject == null) return;
            
            MessageBox.Show($"em xin loi {subject.SubjectName} se {subject.ClassName} se duoc lam sau.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // For future implementation:
            // _navigationService.NavigateToWithParameter(AppViews.SubjectSchedule, subject);
        }
    }

    // Helper class to hold teaching assignment information
    public class SubjectTeachingInfo
    {
        public int TeacherSubjectID { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public int StudentCount { get; set; }
    }
}
