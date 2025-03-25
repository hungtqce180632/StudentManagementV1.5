using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    public class ExamDialogViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly Window _dialogWindow;
        private readonly Exam? _originalExam;
        private readonly bool _isEditMode;
        private bool _isLoading;
        private string _dialogTitle;
        private string _examName = string.Empty;
        private DateTime _examDate = DateTime.Now;
        private int? _duration;
        private int _totalMarks = 100;
        private string _description = string.Empty;
        private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();
        private ObservableCollection<Class> _classes = new ObservableCollection<Class>();
        private Subject? _selectedSubject;
        private Class? _selectedClass;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetProperty(ref _dialogTitle, value);
        }

        public string ExamName
        {
            get => _examName;
            set => SetProperty(ref _examName, value);
        }

        public DateTime ExamDate
        {
            get => _examDate;
            set => SetProperty(ref _examDate, value);
        }

        public int? Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public int TotalMarks
        {
            get => _totalMarks;
            set => SetProperty(ref _totalMarks, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
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

        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set => SetProperty(ref _selectedSubject, value);
        }

        public Class? SelectedClass
        {
            get => _selectedClass;
            set => SetProperty(ref _selectedClass, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ExamDialogViewModel(DatabaseService databaseService, Window dialogWindow, Exam? exam = null)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _originalExam = exam;
            _isEditMode = exam != null;

            // Set dialog title based on mode
            _dialogTitle = _isEditMode ? "Edit Exam" : "Add New Exam";

            // If editing, populate fields with existing exam data
            if (_isEditMode && exam != null)
            {
                _examName = exam.ExamName;
                _examDate = exam.ExamDate;
                _duration = exam.Duration;
                _totalMarks = exam.TotalMarks;
                _description = exam.Description;
            }

            // Commands
            SaveCommand = new RelayCommand(_ => SaveExam());
            CancelCommand = new RelayCommand(_ => _dialogWindow.DialogResult = false);

            // Load data
            InitializeDataAsync();
        }

        private async void InitializeDataAsync()
        {
            try
            {
                IsLoading = true;
                await LoadClassesAsync();
                await LoadSubjectsAsync();

                // If editing, set selected class and subject based on the exam
                if (_isEditMode && _originalExam != null)
                {
                    SelectedClass = Classes.FirstOrDefault(c => c.ClassID == _originalExam.ClassID);
                    SelectedSubject = Subjects.FirstOrDefault(s => s.SubjectID == _originalExam.SubjectID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadClassesAsync()
        {
            string query = "SELECT ClassID, ClassName FROM Classes WHERE IsActive = 1 ORDER BY ClassName";
            var result = await _databaseService.ExecuteQueryAsync(query);

            Classes.Clear();
            foreach (DataRow row in result.Rows)
            {
                Classes.Add(new Class
                {
                    ClassID = Convert.ToInt32(row["ClassID"]),
                    ClassName = row["ClassName"].ToString() ?? string.Empty
                });
            }

            if (Classes.Count > 0)
            {
                SelectedClass = Classes[0];
            }
        }

        private async Task LoadSubjectsAsync()
        {
            string query = "SELECT SubjectID, SubjectName FROM Subjects WHERE IsActive = 1 ORDER BY SubjectName";
            var result = await _databaseService.ExecuteQueryAsync(query);

            Subjects.Clear();
            foreach (DataRow row in result.Rows)
            {
                Subjects.Add(new Subject
                {
                    SubjectID = Convert.ToInt32(row["SubjectID"]),
                    SubjectName = row["SubjectName"].ToString() ?? string.Empty
                });
            }

            if (Subjects.Count > 0)
            {
                SelectedSubject = Subjects[0];
            }
        }

        private async void SaveExam()
        {
            if (!ValidateExam())
            {
                return;
            }

            try
            {
                IsLoading = true;

                if (_isEditMode)
                {
                    await UpdateExamAsync();
                }
                else
                {
                    await CreateExamAsync();
                }

                _dialogWindow.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving exam: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                IsLoading = false;
            }
        }

        private bool ValidateExam()
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(ExamName))
            {
                MessageBox.Show("Please enter an exam name.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedSubject == null)
            {
                MessageBox.Show("Please select a subject.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedClass == null)
            {
                MessageBox.Show("Please select a class.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (TotalMarks <= 0)
            {
                MessageBox.Show("Total marks must be greater than zero.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Duration.HasValue && Duration.Value <= 0)
            {
                MessageBox.Show("Duration must be greater than zero.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private async Task CreateExamAsync()
        {
            // Fixed: Removed CreatedAt column which doesn't exist in the database
            string query = @"
                INSERT INTO Exams (
                    SubjectID, 
                    ClassID, 
                    ExamName, 
                    ExamDate, 
                    Duration, 
                    TotalMarks, 
                    Description
                ) 
                VALUES (
                    @SubjectID, 
                    @ClassID, 
                    @ExamName, 
                    @ExamDate, 
                    @Duration, 
                    @TotalMarks, 
                    @Description
                )";

            var parameters = new Dictionary<string, object>
            {
                { "@SubjectID", SelectedSubject!.SubjectID },
                { "@ClassID", SelectedClass!.ClassID },
                { "@ExamName", ExamName },
                { "@ExamDate", ExamDate },
                { "@TotalMarks", TotalMarks },
                { "@Description", Description ?? string.Empty }
            };

            if (Duration.HasValue)
            {
                parameters.Add("@Duration", Duration.Value);
            }
            else
            {
                parameters.Add("@Duration", DBNull.Value);
            }

            await _databaseService.ExecuteNonQueryAsync(query, parameters);
            MessageBox.Show("Exam created successfully!",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task UpdateExamAsync()
        {
            if (_originalExam == null) return;

            // Fixed: Removed UpdatedAt column which doesn't exist in the database
            string query = @"
                UPDATE Exams 
                SET 
                    SubjectID = @SubjectID,
                    ClassID = @ClassID,
                    ExamName = @ExamName,
                    ExamDate = @ExamDate,
                    Duration = @Duration,
                    TotalMarks = @TotalMarks,
                    Description = @Description
                WHERE 
                    ExamID = @ExamID";

            var parameters = new Dictionary<string, object>
            {
                { "@ExamID", _originalExam.ExamID },
                { "@SubjectID", SelectedSubject!.SubjectID },
                { "@ClassID", SelectedClass!.ClassID },
                { "@ExamName", ExamName },
                { "@ExamDate", ExamDate },
                { "@TotalMarks", TotalMarks },
                { "@Description", Description ?? string.Empty }
            };

            if (Duration.HasValue)
            {
                parameters.Add("@Duration", Duration.Value);
            }
            else
            {
                parameters.Add("@Duration", DBNull.Value);
            }

            await _databaseService.ExecuteNonQueryAsync(query, parameters);
            MessageBox.Show("Exam updated successfully!",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
