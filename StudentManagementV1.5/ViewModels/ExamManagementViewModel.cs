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
using System.Windows.Threading;

namespace StudentManagementV1._5.ViewModels
{
    public class ExamManagementViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly NavigationService _navigationService;

        private ObservableCollection<Exam> _exams = new ObservableCollection<Exam>();
        private ObservableCollection<Class> _classes = new ObservableCollection<Class>();
        private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();
        private string _searchText = string.Empty;
        private bool _isLoading;
        private Exam? _selectedExam;
        private Class? _selectedClass;
        private Subject? _selectedSubject;

        public ObservableCollection<Exam> Exams
        {
            get => _exams;
            set => SetProperty(ref _exams, value);
        }

        public ObservableCollection<Class> Classes
        {
            get => _classes;
            set => SetProperty(ref _classes, value);
        }

        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set => SetProperty(ref _subjects, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = RefreshExamsAsync();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public Exam? SelectedExam
        {
            get => _selectedExam;
            set => SetProperty(ref _selectedExam, value);
        }

        public Class? SelectedClass
        {
            get => _selectedClass;
            set
            {
                if (SetProperty(ref _selectedClass, value))
                {
                    _ = RefreshExamsAsync();
                }
            }
        }

        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                if (SetProperty(ref _selectedSubject, value))
                {
                    _ = RefreshExamsAsync();
                }
            }
        }

        public ICommand AddExamCommand { get; }
        public ICommand EditExamCommand { get; }
        public ICommand DeleteExamCommand { get; }
        public ICommand ViewScoresCommand { get; }
        public ICommand BackCommand { get; }

        public ExamManagementViewModel(DatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;

            AddExamCommand = new RelayCommand(_ => AddExam());
            EditExamCommand = new RelayCommand(param => EditExam(param as Exam), param => param != null);
            DeleteExamCommand = new RelayCommand(async param => await DeleteExamAsync(param as Exam), param => param != null);
            ViewScoresCommand = new RelayCommand(param => ViewScores(param as Exam), param => param != null);
            BackCommand = new RelayCommand(_ => _navigationService.NavigateTo(AppViews.AdminDashboard));
            
            // Load initial data - don't use Task.Run which creates a background thread
            InitializeDataAsync();
        }
        
        // New method to initialize data properly
        private async void InitializeDataAsync()
        {
            try
            {
                await LoadClassesAsync();
                await LoadSubjectsAsync();
                await LoadExamsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing data: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task RefreshExamsAsync()
        {
            await LoadExamsAsync();
        }
        
        private async Task LoadClassesAsync()
        {
            try
            {
                IsLoading = true;
                
                // Load classes from database
                string query = "SELECT ClassID, ClassName FROM Classes WHERE IsActive = 1 ORDER BY ClassName";
                var result = await _databaseService.ExecuteQueryAsync(query);

                // Update UI on the dispatcher thread
                Application.Current.Dispatcher.Invoke(() => {
                    Classes.Clear();
                    // Add "All Classes" option
                    Classes.Add(new Class { ClassID = 0, ClassName = "All Classes" });

                    foreach (DataRow row in result.Rows)
                    {
                        Classes.Add(new Class
                        {
                            ClassID = Convert.ToInt32(row["ClassID"]),
                            ClassName = row["ClassName"].ToString() ?? string.Empty
                        });
                    }

                    // Set default selection
                    SelectedClass = Classes[0]; // "All Classes"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading classes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private async Task LoadSubjectsAsync()
        {
            try
            {
                IsLoading = true;
                
                // Load subjects from database
                string query = "SELECT SubjectID, SubjectName FROM Subjects WHERE IsActive = 1 ORDER BY SubjectName";
                var result = await _databaseService.ExecuteQueryAsync(query);

                // Update UI on the dispatcher thread
                Application.Current.Dispatcher.Invoke(() => {
                    Subjects.Clear();
                    // Add "All Subjects" option
                    Subjects.Add(new Subject { SubjectID = 0, SubjectName = "All Subjects" });

                    foreach (DataRow row in result.Rows)
                    {
                        Subjects.Add(new Subject
                        {
                            SubjectID = Convert.ToInt32(row["SubjectID"]),
                            SubjectName = row["SubjectName"].ToString() ?? string.Empty
                        });
                    }

                    // Set default selection
                    SelectedSubject = Subjects[0]; // "All Subjects"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private async Task LoadExamsAsync()
        {
            try
            {
                IsLoading = true;
                
                string query = BuildExamQuery();
                var parameters = BuildQueryParameters();
                DataTable result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                // Update UI on the dispatcher thread
                Application.Current.Dispatcher.Invoke(() => {
                    Exams.Clear();
                    PopulateExamsFromDataTable(result);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading exams: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private string BuildExamQuery()
        {
            string query = @"
                SELECT 
                    e.ExamID, 
                    e.ExamName, 
                    e.SubjectID,
                    s.SubjectName,
                    e.ClassID,
                    c.ClassName,
                    e.ExamDate, 
                    e.Duration, 
                    e.TotalMarks, 
                    e.Description
                FROM Exams e
                JOIN Subjects s ON e.SubjectID = s.SubjectID
                JOIN Classes c ON e.ClassID = c.ClassID
                WHERE 1=1";
            
            if (_selectedClass?.ClassID > 0)
            {
                query += " AND e.ClassID = @ClassID";
            }

            if (_selectedSubject?.SubjectID > 0)
            {
                query += " AND e.SubjectID = @SubjectID";
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                query += " AND (e.ExamName LIKE @Search OR e.Description LIKE @Search)";
            }

            query += " ORDER BY e.ExamDate DESC";
            
            return query;
        }
        
        private Dictionary<string, object> BuildQueryParameters()
        {
            var parameters = new Dictionary<string, object>();
            
            if (_selectedClass?.ClassID > 0)
            {
                parameters["@ClassID"] = _selectedClass.ClassID;
            }
            
            if (_selectedSubject?.SubjectID > 0)
            {
                parameters["@SubjectID"] = _selectedSubject.SubjectID;
            }
            
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                parameters["@Search"] = $"%{_searchText}%";
            }
            
            return parameters;
        }
        
        private void PopulateExamsFromDataTable(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Exams.Add(new Exam
                {
                    ExamID = Convert.ToInt32(row["ExamID"]),
                    ExamName = row["ExamName"].ToString() ?? string.Empty,
                    SubjectID = Convert.ToInt32(row["SubjectID"]),
                    SubjectName = row["SubjectName"].ToString() ?? string.Empty,
                    ClassID = Convert.ToInt32(row["ClassID"]),
                    ClassName = row["ClassName"].ToString() ?? string.Empty,
                    ExamDate = Convert.ToDateTime(row["ExamDate"]),
                    Duration = row["Duration"] != DBNull.Value ? Convert.ToInt32(row["Duration"]) : null,
                    TotalMarks = Convert.ToInt32(row["TotalMarks"]),
                    Description = row["Description"]?.ToString() ?? string.Empty
                });
            }
        }
        
        private void AddExam()
        {
            // For now just show a placeholder message
            // In a future update, implement an Add Exam dialog
            MessageBox.Show("Add Exam functionality will be implemented in a future update.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void EditExam(Exam? exam)
        {
            if (exam == null) return;
            
            // For now just show a placeholder message
            // In a future update, implement an Edit Exam dialog
            MessageBox.Show($"Edit Exam '{exam.ExamName}' functionality will be implemented in a future update.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private async Task DeleteExamAsync(Exam? exam)
        {
            if (exam == null) return;
            
            // Confirm deletion
            var result = MessageBox.Show($"Are you sure you want to delete exam '{exam.ExamName}'?\n\nThis will also delete all associated scores.", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                await DeleteExamFromDatabaseAsync(exam);
            }
        }
        
        private async Task DeleteExamFromDatabaseAsync(Exam exam)
        {
            try
            {
                IsLoading = true;
                
                // Check if exam has scores
                string checkQuery = "SELECT COUNT(*) FROM Scores WHERE ExamID = @ExamID";
                var checkParams = new Dictionary<string, object> { { "@ExamID", exam.ExamID } };
                var scoreCount = await _databaseService.ExecuteScalarAsync(checkQuery, checkParams);
                
                if (scoreCount != null && Convert.ToInt32(scoreCount) > 0)
                {
                    var confirmDelete = MessageBox.Show(
                        $"This exam has {scoreCount} score records associated with it. Deleting the exam will also delete all scores. Continue?",
                        "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    
                    if (confirmDelete != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    
                    // Delete associated scores first
                    string deleteScoresQuery = "DELETE FROM Scores WHERE ExamID = @ExamID";
                    await _databaseService.ExecuteNonQueryAsync(deleteScoresQuery, checkParams);
                }
                
                // Delete the exam
                string query = "DELETE FROM Exams WHERE ExamID = @ExamID";
                var parameters = new Dictionary<string, object>
                {
                    { "@ExamID", exam.ExamID }
                };
                
                await _databaseService.ExecuteNonQueryAsync(query, parameters);
                
                // Refresh the list
                await LoadExamsAsync();

                MessageBox.Show($"Exam '{exam.ExamName}' has been deleted.", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting exam: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private void ViewScores(Exam? exam)
        {
            if (exam == null) return;
            
            // Navigate to the score management view with the selected exam
            MessageBox.Show($"View scores for '{exam.ExamName}' functionality will be implemented in a future update.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
                
            // In future implementation:
            // _navigationService.NavigateTo(AppViews.ScoreManagement, exam);
        }
    }
}
