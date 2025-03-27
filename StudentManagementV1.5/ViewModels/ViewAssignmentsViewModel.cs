using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp ViewAssignmentsViewModel
    // + Tại sao cần sử dụng: Hiển thị danh sách bài tập cho học sinh
    // + Lớp này được gọi từ màn hình dashboard của học sinh
    // + Chức năng chính: Hiển thị, tìm kiếm, lọc bài tập và cho phép học sinh nộp bài
    public class ViewAssignmentsViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly NavigationService _navigationService;
        private readonly AuthenticationService _authService;
        
        private ObservableCollection<Assignment> _assignments = new ObservableCollection<Assignment>();
        private ObservableCollection<string> _subjects = new ObservableCollection<string>();
        private Assignment _selectedAssignment;
        private string _selectedSubject = "All Subjects";
        private string _searchText = string.Empty;
        private string _filterStatus = "All";
        private string _errorMessage = string.Empty;
        private bool _isLoading;
        
        // Properties
        public ObservableCollection<Assignment> Assignments
        {
            get => _assignments;
            set => SetProperty(ref _assignments, value);
        }
        
        public ObservableCollection<string> Subjects
        {
            get => _subjects;
            set => SetProperty(ref _subjects, value);
        }
        
        public Assignment SelectedAssignment
        {
            get => _selectedAssignment;
            set => SetProperty(ref _selectedAssignment, value);
        }
        
        public string SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                if (SetProperty(ref _selectedSubject, value))
                {
                    RefreshAssignmentsAsync();
                }
            }
        }
        
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    RefreshAssignmentsAsync();
                }
            }
        }
        
        public string FilterStatus
        {
            get => _filterStatus;
            set
            {
                if (SetProperty(ref _filterStatus, value))
                {
                    RefreshAssignmentsAsync();
                }
            }
        }
        
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        
        public bool HasAssignments => Assignments.Count > 0;
        
        // Filter options for assignment status
        public List<string> StatusOptions { get; } = new List<string> { "All", "Upcoming", "Due Soon", "Submitted", "Overdue" };
        
        // Commands
        public ICommand BackCommand { get; }
        public ICommand SubmitAssignmentCommand { get; }
        public ICommand ViewAssignmentDetailsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        
        // Constructor
        public ViewAssignmentsViewModel(DatabaseService databaseService, NavigationService navigationService, AuthenticationService authService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _authService = authService;
            
            // Initialize commands
            BackCommand = new RelayCommand(_ => _navigationService.NavigateTo(AppViews.StudentDashboard));
            SubmitAssignmentCommand = new RelayCommand(param => SubmitAssignment(param as Assignment), param => param != null);
            ViewAssignmentDetailsCommand = new RelayCommand(param => ViewAssignmentDetails(param as Assignment), param => param != null);
            RefreshCommand = new RelayCommand(_ => RefreshAssignmentsAsync());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            
            // Load initial data
            LoadInitialDataAsync();
        }
        
        // Load subjects and assignments
        private async void LoadInitialDataAsync()
        {
            await LoadSubjectsAsync();
            await LoadAssignmentsAsync();
        }
        
        // Load subjects for the current student
        private async Task LoadSubjectsAsync()
        {
            try
            {
                IsLoading = true;
                Subjects.Clear();
                
                // Add "All Subjects" option
                Subjects.Add("All Subjects");
                
                // Get student ID
                int studentId = _authService.CurrentUser?.UserID ?? 0;
                if (studentId == 0) return;
                
                string query = @"
                    SELECT DISTINCT s.SubjectName
                    FROM Subjects s
                    JOIN Classes c ON s.SubjectID = c.SubjectID
                    JOIN Students st ON c.ClassID = st.ClassID
                    WHERE st.StudentID = @StudentID
                    ORDER BY s.SubjectName";
                
                var parameters = new Dictionary<string, object>
                {
                    { "@StudentID", studentId }
                };
                
                var result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                foreach (DataRow row in result.Rows)
                {
                    Subjects.Add(row["SubjectName"].ToString());
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading subjects: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        // Load assignments for the current student
        private async Task LoadAssignmentsAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                Assignments.Clear();
                
                // Get student ID
                int studentId = _authService.CurrentUser?.UserID ?? 0;
                if (studentId == 0)
                {
                    ErrorMessage = "Student information not found. Please log in again.";
                    return;
                }
                
                // Build the query with filters
                string query = @"
                    SELECT a.AssignmentID, a.Title, a.Description, a.AssignedDate AS CreatedDate, 
                           a.Deadline AS DueDate, a.TotalMarks AS MaxPoints, 
                           a.TeacherID, a.SubjectID, a.ClassID, a.Status,
                           s.SubjectName, c.ClassName, t.FirstName + ' ' + t.LastName AS TeacherName,
                           CASE WHEN sub.SubmissionID IS NULL THEN 0 ELSE 1 END AS HasSubmitted,
                           sub.Status AS SubmissionStatus, sub.MarksObtained AS Score
                    FROM Assignments a
                    JOIN Subjects s ON a.SubjectID = s.SubjectID
                    JOIN Classes c ON a.ClassID = c.ClassID
                    JOIN Teachers t ON a.TeacherID = t.TeacherID
                    JOIN Students st ON c.ClassID = st.ClassID
                    LEFT JOIN Submissions sub ON a.AssignmentID = sub.AssignmentID AND sub.StudentID = @StudentID
                    WHERE st.StudentID = @StudentID AND a.Status = 'Published'";
                
                var parameters = new Dictionary<string, object>
                {
                    { "@StudentID", studentId }
                };
                
                // Add subject filter if not "All Subjects"
                if (_selectedSubject != "All Subjects")
                {
                    query += " AND s.SubjectName = @SubjectName";
                    parameters.Add("@SubjectName", _selectedSubject);
                }
                
                // Add status filter if not "All"
                if (_filterStatus != "All")
                {
                    if (_filterStatus == "Submitted")
                    {
                        query += " AND sub.SubmissionID IS NOT NULL";
                    }
                    else if (_filterStatus == "Upcoming" || _filterStatus == "Due Soon" || _filterStatus == "Overdue")
                    {
                        // We'll filter these client-side since they depend on the current time
                    }
                }
                
                // Add search filter if provided
                if (!string.IsNullOrWhiteSpace(_searchText))
                {
                    query += " AND (a.Title LIKE @Search OR a.Description LIKE @Search OR s.SubjectName LIKE @Search)";
                    parameters.Add("@Search", $"%{_searchText}%");
                }
                
                // Order by due date (soonest first)
                query += " ORDER BY a.Deadline ASC";
                
                var result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                foreach (DataRow row in result.Rows)
                {
                    var assignment = new Assignment
                    {
                        AssignmentID = Convert.ToInt32(row["AssignmentID"]),
                        Title = row["Title"].ToString() ?? string.Empty,
                        Description = row["Description"].ToString() ?? string.Empty,
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        DueDate = Convert.ToDateTime(row["DueDate"]),
                        MaxPoints = Convert.ToInt32(row["MaxPoints"]),
                        TeacherID = Convert.ToInt32(row["TeacherID"]),
                        TeacherName = row["TeacherName"].ToString() ?? string.Empty,
                        SubjectID = Convert.ToInt32(row["SubjectID"]),
                        SubjectName = row["SubjectName"].ToString() ?? string.Empty,
                        ClassID = Convert.ToInt32(row["ClassID"]),
                        ClassName = row["ClassName"].ToString() ?? string.Empty,
                        Status = row["Status"].ToString() ?? "Draft"
                    };
                    
                    // Set additional properties based on submission status
                    bool hasSubmitted = Convert.ToBoolean(row["HasSubmitted"]);
                    assignment.SubmissionStatus = hasSubmitted ? row["SubmissionStatus"].ToString() : "Not Submitted";
                    assignment.Score = hasSubmitted && !Convert.IsDBNull(row["Score"]) ? Convert.ToInt32(row["Score"]) : null;
                    
                    // Filter by status (client-side for time-dependent statuses)
                    string dueStatus = assignment.GetDueStatus();
                    if (_filterStatus == "All" || 
                        (_filterStatus == "Upcoming" && dueStatus == "Upcoming") ||
                        (_filterStatus == "Due Soon" && dueStatus == "Due Soon") ||
                        (_filterStatus == "Overdue" && dueStatus == "Overdue") ||
                        (_filterStatus == "Submitted" && hasSubmitted))
                    {
                        Assignments.Add(assignment);
                    }
                }
                
                if (Assignments.Count == 0)
                {
                    ErrorMessage = "No assignments found matching your filters.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading assignments: {ex.Message}";
                MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        // Refresh assignments based on current filters
        private async void RefreshAssignmentsAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                await LoadAssignmentsAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        // Clear all filters
        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedSubject = "All Subjects";
            FilterStatus = "All";
        }
        
        // View details of an assignment
        private void ViewAssignmentDetails(Assignment assignment)
        {
            if (assignment == null) return;
            
            // TODO: Navigation to assignment details view needs to be implemented
            // For now, show assignment details in a message box as temporary solution
            MessageBox.Show(
                $"Assignment: {assignment.Title}\n\n" +
                $"Subject: {assignment.SubjectName}\n" +
                $"Teacher: {assignment.TeacherName}\n" +
                $"Due Date: {assignment.DueDate:yyyy-MM-dd HH:mm}\n" +
                $"Status: {assignment.Status}\n" +
                $"Description: {assignment.Description}",
                "Assignment Details",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            
            // When AppViews.AssignmentDetails is properly defined in the enumeration:
            // _navigationService.NavigateToWithParameter(AppViews.AssignmentDetails, assignment);
        }
        
        // Submit work for an assignment
        private void SubmitAssignment(Assignment assignment)
        {
            if (assignment == null) return;
            
            // TODO: Navigation to submission view needs to be implemented
            // For now, show a placeholder message
            MessageBox.Show(
                $"Submission functionality for '{assignment.Title}' will be available soon.\n\n" +
                $"Due Date: {assignment.DueDate:yyyy-MM-dd HH:mm}\n" +
                $"Current Status: {assignment.SubmissionStatus}",
                "Submit Assignment",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            
            // When AppViews.SubmitAssignment is properly defined in the enumeration:
            // _navigationService.NavigateToWithParameter(AppViews.SubmitAssignment, assignment);
        }
    }
}
