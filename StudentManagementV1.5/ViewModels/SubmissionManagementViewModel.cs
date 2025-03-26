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
    // Lớp SubmissionManagementViewModel
    // + Tại sao cần sử dụng: Quản lý và hiển thị danh sách bài nộp của học sinh
    // + Lớp này được gọi từ màn hình quản lý bài nộp của giáo viên
    // + Chức năng chính: Xem, chấm điểm và đưa phản hồi cho bài nộp của học sinh
    public class SubmissionManagementViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly NavigationService _navigationService;
        private readonly AuthenticationService _authService;
        
        private Assignment _assignment;
        private ObservableCollection<Submission> _submissions = new ObservableCollection<Submission>();
        private Submission _selectedSubmission;
        private string _errorMessage = string.Empty;
        private bool _isLoading;
        private string _filterStatus = "All";
        private string _searchText = string.Empty;
        
        // Properties
        public Assignment Assignment
        {
            get => _assignment;
            set => SetProperty(ref _assignment, value);
        }
        
        public ObservableCollection<Submission> Submissions
        {
            get => _submissions;
            set => SetProperty(ref _submissions, value);
        }
        
        public Submission SelectedSubmission
        {
            get => _selectedSubmission;
            set => SetProperty(ref _selectedSubmission, value);
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
                    RefreshSubmissionsAsync();
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
                    RefreshSubmissionsAsync();
                }
            }
        }
        
        // Filter options for submission status
        public List<string> StatusOptions { get; } = new List<string> { "All", "Submitted", "Graded", "Rejected" };
        
        // Commands
        public ICommand BackCommand { get; }
        public ICommand GradeSubmissionCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        
        // Constructor
        public SubmissionManagementViewModel(DatabaseService databaseService, NavigationService navigationService, AuthenticationService authService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _authService = authService;
            
            // Try to get the assignment from navigation parameter
            Assignment = (Assignment)_navigationService.GetAndClearParameter();
            
            if (Assignment == null)
            {
                ErrorMessage = "No assignment selected. Please go back and select an assignment.";
            }
            
            // Initialize commands
            BackCommand = new RelayCommand(_ => _navigationService.NavigateTo(AppViews.AssignmentManagement));
            GradeSubmissionCommand = new RelayCommand(param => GradeSubmission(param as Submission), param => param != null);
            RefreshCommand = new RelayCommand(_ => RefreshSubmissionsAsync());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            
            // Load submissions if assignment is valid
            if (Assignment != null)
            {
                LoadSubmissionsAsync();
            }
        }
        
        // Load submissions for the current assignment
        private async void LoadSubmissionsAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                Submissions.Clear();
                
                if (Assignment == null) return;
                
                // Build the query with filters
                string query = @"
                    SELECT s.SubmissionID, s.AssignmentID, s.StudentID, s.Content, s.AttachmentPath, 
                           s.SubmissionDate, s.Status, s.Score, s.Feedback,
                           CONCAT(st.FirstName, ' ', st.LastName) AS StudentName
                    FROM Submissions s
                    JOIN Students st ON s.StudentID = st.StudentID
                    WHERE s.AssignmentID = @AssignmentID";
                
                var parameters = new Dictionary<string, object>
                {
                    { "@AssignmentID", Assignment.AssignmentID }
                };
                
                // Add status filter if not "All"
                if (FilterStatus != "All")
                {
                    query += " AND s.Status = @Status";
                    parameters.Add("@Status", FilterStatus);
                }
                
                // Add search filter if provided
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query += " AND (s.Content LIKE @Search OR CONCAT(st.FirstName, ' ', st.LastName) LIKE @Search)";
                    parameters.Add("@Search", $"%{SearchText}%");
                }
                
                // Order by submission date (most recent first)
                query += " ORDER BY s.SubmissionDate DESC";
                
                var result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                foreach (DataRow row in result.Rows)
                {
                    Submissions.Add(new Submission
                    {
                        SubmissionID = Convert.ToInt32(row["SubmissionID"]),
                        AssignmentID = Convert.ToInt32(row["AssignmentID"]),
                        AssignmentTitle = Assignment.Title,
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        StudentName = row["StudentName"].ToString() ?? string.Empty,
                        Content = row["Content"].ToString() ?? string.Empty,
                        AttachmentPath = row["AttachmentPath"].ToString() ?? string.Empty,
                        SubmissionDate = Convert.ToDateTime(row["SubmissionDate"]),
                        Status = row["Status"].ToString() ?? "Submitted",
                        Score = row["Score"] != DBNull.Value ? Convert.ToInt32(row["Score"]) : null,
                        Feedback = row["Feedback"].ToString() ?? string.Empty,
                        DueDate = Assignment.DueDate // This should be correct as it comes from the Assignment object
                    });
                }
                
                if (Submissions.Count == 0)
                {
                    ErrorMessage = "No submissions found for this assignment.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading submissions: {ex.Message}";
                MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        // Refresh submissions based on current filters
        private async void RefreshSubmissionsAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                await Task.Run(() => LoadSubmissionsAsync());
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
            FilterStatus = "All";
        }
        
        // Grade a submission
        private void GradeSubmission(Submission submission)
        {
            if (submission == null) return;
            
            var dialog = new SubmissionGradeDialogView();
            var viewModel = new SubmissionGradeViewModel(_databaseService, dialog, Assignment, submission);
            dialog.DataContext = viewModel;
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true)
            {
                RefreshSubmissionsAsync();
            }
        }
    }
}
