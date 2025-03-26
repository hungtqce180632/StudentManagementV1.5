using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp SubmissionGradeViewModel
    // + Tại sao cần sử dụng: Xử lý logic cho việc chấm điểm bài nộp
    // + Lớp này được sử dụng trong hộp thoại chấm điểm bài nộp
    // + Chức năng chính: Chấm điểm và đưa phản hồi cho bài nộp của học sinh
    public class SubmissionGradeViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly Window _dialogWindow;
        private readonly Assignment _assignment;
        private readonly Submission _originalSubmission;
        
        private Submission _submission;
        private bool _isProcessing;
        private string _scoreError = string.Empty;
        
        // Properties
        public Submission Submission
        {
            get => _submission;
            set => SetProperty(ref _submission, value);
        }
        
        public Assignment Assignment
        {
            get => _assignment;
        }
        
        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }
        
        public string ScoreError
        {
            get => _scoreError;
            set => SetProperty(ref _scoreError, value);
        }
        
        public bool HasScoreError => !string.IsNullOrEmpty(ScoreError);
        
        // List of possible submission statuses
        public List<string> StatusOptions { get; } = new List<string> { "Submitted", "Graded", "Rejected" };
        
        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        
        // Constructor
        public SubmissionGradeViewModel(DatabaseService databaseService, Window dialogWindow, Assignment assignment, Submission submission)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _assignment = assignment;
            _originalSubmission = submission;
            
            // Create a copy of the submission to edit
            _submission = new Submission
            {
                SubmissionID = submission.SubmissionID,
                AssignmentID = submission.AssignmentID,
                AssignmentTitle = submission.AssignmentTitle,
                StudentID = submission.StudentID,
                StudentName = submission.StudentName,
                Content = submission.Content,
                AttachmentPath = submission.AttachmentPath,
                SubmissionDate = submission.SubmissionDate,
                Status = submission.Status,
                Score = submission.Score,
                Feedback = submission.Feedback,
                DueDate = submission.DueDate
            };
            
            // Initialize commands
            SaveCommand = new RelayCommand(async _ => await SaveGradeAsync(), _ => CanSaveGrade());
            CancelCommand = new RelayCommand(_ => CloseDialog(false));
        }
        
        // Check if the submission can be saved
        private bool CanSaveGrade()
        {
            ValidateScore();
            return !HasScoreError;
        }
        
        // Save the grade to the database
        private async Task SaveGradeAsync()
        {
            try
            {
                if (!CanSaveGrade()) return;
                
                IsProcessing = true;
                
                // Update submission in database
                string query = @"
                    UPDATE Submissions
                    SET Status = @Status,
                        Score = @Score,
                        Feedback = @Feedback
                    WHERE SubmissionID = @SubmissionID";
                
                var parameters = new Dictionary<string, object>
                {
                    { "@SubmissionID", _submission.SubmissionID },
                    { "@Status", _submission.Status },
                    { "@Score", _submission.Score.HasValue ? (object)_submission.Score.Value : DBNull.Value },
                    { "@Feedback", _submission.Feedback ?? string.Empty }
                };
                
                await _databaseService.ExecuteNonQueryAsync(query, parameters);
                
                MessageBox.Show("Submission graded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                CloseDialog(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving grade: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }
        
        // Close the dialog
        private void CloseDialog(bool result)
        {
            _dialogWindow.DialogResult = result;
            _dialogWindow.Close();
        }
        
        // Validate the score
        private void ValidateScore()
        {
            if (_submission.Status == "Graded" && (!_submission.Score.HasValue || _submission.Score < 0))
            {
                ScoreError = "Please enter a valid score";
                return;
            }
            
            if (_submission.Score.HasValue && _submission.Score > _assignment.MaxPoints)
            {
                ScoreError = $"Score cannot exceed maximum points ({_assignment.MaxPoints})";
                return;
            }
            
            ScoreError = string.Empty;
        }
    }
}
