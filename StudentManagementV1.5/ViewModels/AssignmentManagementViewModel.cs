using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
using StudentManagementV1._5.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp AssignmentManagementViewModel
    // + Tại sao cần sử dụng: Quản lý và hiển thị danh sách bài tập giáo viên đã giao
    // + Lớp này được gọi từ màn hình quản lý bài tập của giáo viên
    // + Chức năng chính: Thêm, xóa, chỉnh sửa bài tập và quản lý nộp bài
    public class AssignmentManagementViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly NavigationService _navigationService;
        private readonly AuthenticationService _authService;
        
        private ObservableCollection<Assignment> _assignments = new ObservableCollection<Assignment>();
        private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();
        private ObservableCollection<Class> _classes = new ObservableCollection<Class>();
        
        private Assignment _selectedAssignment;
        private Subject _selectedSubject;
        private Class _selectedClass;
        private string _searchText = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading;
        private string _filterStatus = "All";

        // Properties
        public ObservableCollection<Assignment> Assignments
        {
            get => _assignments;
            set => SetProperty(ref _assignments, value);
        }

        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set => SetProperty(ref _subjects, value);
        }

        public ObservableCollection<Class> Classes
        {
            get => _classes;
            set => SetProperty(ref _classes, value);
        }

        public Assignment SelectedAssignment
        {
            get => _selectedAssignment;
            set => SetProperty(ref _selectedAssignment, value);
        }

        public Subject SelectedSubject
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

        public Class SelectedClass
        {
            get => _selectedClass;
            set
            {
                if (SetProperty(ref _selectedClass, value))
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

        // Filter options for assignment status
        public List<string> StatusOptions { get; } = new List<string> { "All", "Draft", "Published", "Closed" };

        // Commands
        public ICommand BackCommand { get; }
        public ICommand CreateAssignmentCommand { get; }
        public ICommand EditAssignmentCommand { get; }
        public ICommand DeleteAssignmentCommand { get; }
        public ICommand ViewSubmissionsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        // Constructor
        public AssignmentManagementViewModel(DatabaseService databaseService, NavigationService navigationService, AuthenticationService authService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _authService = authService;

            // Initialize commands
            BackCommand = new RelayCommand(_ => _navigationService.NavigateTo(AppViews.TeacherDashboard));
            CreateAssignmentCommand = new RelayCommand(_ => CreateAssignment());
            EditAssignmentCommand = new RelayCommand(param => EditAssignment(param as Assignment), param => param != null);
            DeleteAssignmentCommand = new RelayCommand(async param => await DeleteAssignmentAsync(param as Assignment), param => param != null);
            ViewSubmissionsCommand = new RelayCommand(param => ViewSubmissions(param as Assignment), param => param != null);
            RefreshCommand = new RelayCommand(_ => RefreshAssignmentsAsync());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());

            // Load data
            LoadInitialDataAsync();
        }

        // Load initial data - subjects, classes, and assignments
        private async void LoadInitialDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Load all data concurrently
                await Task.WhenAll(
                    LoadSubjectsAsync(),
                    LoadClassesAsync(),
                    LoadAssignmentsAsync()
                );
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
                MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Load subjects taught by this teacher
        private async Task LoadSubjectsAsync()
        {
            Subjects.Clear();
            
            // Add "All Subjects" option
            Subjects.Add(new Subject { SubjectID = 0, SubjectName = "All Subjects" });
            
            // Get teacher ID
            int teacherId = _authService.CurrentUser?.UserID ?? 0;
            if (teacherId == 0) return;

            string query = @"
                SELECT DISTINCT s.SubjectID, s.SubjectName
                FROM Subjects s
                JOIN TeacherSubjects ts ON s.SubjectID = ts.SubjectID
                WHERE ts.TeacherID = @TeacherID AND s.IsActive = 1
                ORDER BY s.SubjectName";

            var parameters = new Dictionary<string, object>
            {
                { "@TeacherID", teacherId }
            };

            var result = await _databaseService.ExecuteQueryAsync(query, parameters);
            
            foreach (DataRow row in result.Rows)
            {
                Subjects.Add(new Subject
                {
                    SubjectID = Convert.ToInt32(row["SubjectID"]),
                    SubjectName = row["SubjectName"].ToString() ?? string.Empty
                });
            }

            // Select "All Subjects" by default
            SelectedSubject = Subjects[0];
        }

        // Load classes taught by this teacher
        private async Task LoadClassesAsync()
        {
            Classes.Clear();
            
            // Add "All Classes" option
            Classes.Add(new Class { ClassID = 0, ClassName = "All Classes" });
            
            // Get teacher ID
            int teacherId = _authService.CurrentUser?.UserID ?? 0;
            if (teacherId == 0) return;

            string query = @"
                SELECT DISTINCT c.ClassID, c.ClassName
                FROM Classes c
                JOIN TeacherSubjects ts ON c.ClassID = ts.ClassID
                WHERE ts.TeacherID = @TeacherID AND c.IsActive = 1
                ORDER BY c.ClassName";

            var parameters = new Dictionary<string, object>
            {
                { "@TeacherID", teacherId }
            };

            var result = await _databaseService.ExecuteQueryAsync(query, parameters);
            
            foreach (DataRow row in result.Rows)
            {
                Classes.Add(new Class
                {
                    ClassID = Convert.ToInt32(row["ClassID"]),
                    ClassName = row["ClassName"].ToString() ?? string.Empty
                });
            }

            // Select "All Classes" by default
            SelectedClass = Classes[0];
        }

        // Load assignments created by this teacher
        private async Task LoadAssignmentsAsync()
        {
            try
            {
                Assignments.Clear();
                
                // Get teacher ID
                int teacherId = _authService.CurrentUser?.UserID ?? 0;
                if (teacherId == 0)
                {
                    ErrorMessage = "Teacher information not found. Please log in again.";
                    return;
                }

                // Build the query with filters - updated to match actual database column names
                string query = @"
                    SELECT a.AssignmentID, a.Title, a.Description, a.AssignedDate AS CreatedDate, 
                           a.Deadline AS DueDate, a.TotalMarks AS MaxPoints, 
                           a.TeacherID, a.SubjectID, a.ClassID, a.Status,
                           s.SubjectName, c.ClassName,
                           (SELECT COUNT(*) FROM Submissions WHERE AssignmentID = a.AssignmentID) AS SubmissionCount
                    FROM Assignments a
                    JOIN Subjects s ON a.SubjectID = s.SubjectID
                    JOIN Classes c ON a.ClassID = c.ClassID
                    WHERE a.TeacherID = @TeacherID";

                var parameters = new Dictionary<string, object>
                {
                    { "@TeacherID", teacherId }
                };

                // Add subject filter if selected
                if (SelectedSubject != null && SelectedSubject.SubjectID > 0)
                {
                    query += " AND a.SubjectID = @SubjectID";
                    parameters.Add("@SubjectID", SelectedSubject.SubjectID);
                }

                // Add class filter if selected
                if (SelectedClass != null && SelectedClass.ClassID > 0)
                {
                    query += " AND a.ClassID = @ClassID";
                    parameters.Add("@ClassID", SelectedClass.ClassID);
                }

                // Add status filter if not "All"
                if (FilterStatus != "All")
                {
                    query += " AND a.Status = @Status";
                    parameters.Add("@Status", FilterStatus);
                }

                // Add search filter if provided
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query += " AND (a.Title LIKE @Search OR a.Description LIKE @Search)";
                    parameters.Add("@Search", $"%{SearchText}%");
                }

                // Order by deadline (most recent first) - FIX HERE: Use Deadline instead of DueDate
                query += " ORDER BY a.Deadline DESC";

                var result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                foreach (DataRow row in result.Rows)
                {
                    Assignments.Add(new Assignment
                    {
                        AssignmentID = Convert.ToInt32(row["AssignmentID"]),
                        Title = row["Title"].ToString() ?? string.Empty,
                        Description = row["Description"].ToString() ?? string.Empty,
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        DueDate = Convert.ToDateTime(row["DueDate"]),
                        MaxPoints = Convert.ToInt32(row["MaxPoints"]),
                        TeacherID = Convert.ToInt32(row["TeacherID"]),
                        SubjectID = Convert.ToInt32(row["SubjectID"]),
                        SubjectName = row["SubjectName"].ToString() ?? string.Empty,
                        ClassID = Convert.ToInt32(row["ClassID"]),
                        ClassName = row["ClassName"].ToString() ?? string.Empty,
                        Status = row["Status"].ToString() ?? "Draft",
                        SubmissionCount = Convert.ToInt32(row["SubmissionCount"])
                    });
                }

                if (Assignments.Count == 0)
                {
                    ErrorMessage = "No assignments found. Create a new assignment to get started.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading assignments: {ex.Message}";
                MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            SelectedSubject = Subjects[0]; // "All Subjects"
            SelectedClass = Classes[0];    // "All Classes"
            FilterStatus = "All";
        }

        // Create a new assignment
        private void CreateAssignment()
        {
            var dialog = new AssignmentDialogView();
            var viewModel = new AssignmentDialogViewModel(_databaseService, dialog, _authService);
            dialog.DataContext = viewModel;
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true)
            {
                RefreshAssignmentsAsync();
            }
        }

        // Edit an existing assignment
        private void EditAssignment(Assignment assignment)
        {
            if (assignment == null) return;
            
            var dialog = new AssignmentDialogView();
            var viewModel = new AssignmentDialogViewModel(_databaseService, dialog, _authService, assignment);
            dialog.DataContext = viewModel;
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true)
            {
                RefreshAssignmentsAsync();
            }
        }

        // Delete an assignment
        private async Task DeleteAssignmentAsync(Assignment assignment)
        {
            if (assignment == null) return;

            // Confirm deletion
            var result = MessageBox.Show(
                $"Are you sure you want to delete the assignment '{assignment.Title}'?\n\nThis action cannot be undone.",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;

                // Check if there are any submissions for this assignment
                string checkQuery = "SELECT COUNT(*) FROM Submissions WHERE AssignmentID = @AssignmentID";
                var checkParams = new Dictionary<string, object>
                {
                    { "@AssignmentID", assignment.AssignmentID }
                };

                var submissionCount = await _databaseService.ExecuteScalarAsync(checkQuery, checkParams);
                
                if (submissionCount != null && Convert.ToInt32(submissionCount) > 0)
                {
                    // There are submissions, so we shouldn't physically delete the assignment
                    // Instead, we'll mark it as "Closed" or inactive
                    string updateQuery = "UPDATE Assignments SET Status = 'Closed' WHERE AssignmentID = @AssignmentID";
                    await _databaseService.ExecuteNonQueryAsync(updateQuery, checkParams);
                    
                    MessageBox.Show(
                        "This assignment has submissions, so it has been closed instead of deleted. It will no longer be visible to students.",
                        "Assignment Closed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    // No submissions, we can safely delete
                    string deleteQuery = "DELETE FROM Assignments WHERE AssignmentID = @AssignmentID";
                    await _databaseService.ExecuteNonQueryAsync(deleteQuery, checkParams);
                    
                    MessageBox.Show(
                        "Assignment has been deleted successfully.",
                        "Deletion Successful",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                // Refresh the assignments list
                await LoadAssignmentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error deleting assignment: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // View submissions for an assignment
        private void ViewSubmissions(Assignment assignment)
        {
            if (assignment == null) return;
            
            // Navigate to the submissions view with the selected assignment
            _navigationService.NavigateToWithParameter(AppViews.SubmissionManagement, assignment);
        }
    }
}
