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
    // Lớp AssignmentDialogViewModel
    // + Tại sao cần sử dụng: Xử lý logic cho hộp thoại thêm/sửa bài tập
    // + Lớp này được sử dụng trong hộp thoại thêm/sửa bài tập
    // + Chức năng chính: Xác thực, lưu và cập nhật thông tin bài tập
    public class AssignmentDialogViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly Window _dialogWindow;
        private readonly AuthenticationService _authService;
        private readonly Assignment _originalAssignment;
        private bool _isEditMode;
        private bool _isProcessing;
        
        private Assignment _assignment;
        private ObservableCollection<Subject> _availableSubjects = new ObservableCollection<Subject>();
        private ObservableCollection<Class> _availableClasses = new ObservableCollection<Class>();
        private Subject _selectedSubject;
        private Class _selectedClass;
        
        private string _titleError = string.Empty;
        private string _dueDateError = string.Empty;
        private string _maxPointsError = string.Empty;
        private string _subjectError = string.Empty;
        private string _classError = string.Empty;

        // Properties
        public string DialogTitle => _isEditMode ? "Edit Assignment" : "Create New Assignment";
        
        public Assignment Assignment
        {
            get => _assignment;
            set => SetProperty(ref _assignment, value);
        }
        
        public ObservableCollection<Subject> AvailableSubjects
        {
            get => _availableSubjects;
            set => SetProperty(ref _availableSubjects, value);
        }
        
        public ObservableCollection<Class> AvailableClasses
        {
            get => _availableClasses;
            set => SetProperty(ref _availableClasses, value);
        }
        
        public Subject SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                if (SetProperty(ref _selectedSubject, value))
                {
                    ValidateSubject();
                    if (value != null)
                    {
                        Assignment.SubjectID = value.SubjectID;
                        Assignment.SubjectName = value.SubjectName;
                    }
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
                    ValidateClass();
                    if (value != null)
                    {
                        Assignment.ClassID = value.ClassID;
                        Assignment.ClassName = value.ClassName;
                    }
                }
            }
        }
        
        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }
        
        public string TitleError
        {
            get => _titleError;
            set => SetProperty(ref _titleError, value);
        }
        
        public bool HasTitleError => !string.IsNullOrEmpty(TitleError);
        
        public string DueDateError
        {
            get => _dueDateError;
            set => SetProperty(ref _dueDateError, value);
        }
        
        public bool HasDueDateError => !string.IsNullOrEmpty(DueDateError);
        
        public string MaxPointsError
        {
            get => _maxPointsError;
            set => SetProperty(ref _maxPointsError, value);
        }
        
        public bool HasMaxPointsError => !string.IsNullOrEmpty(MaxPointsError);
        
        public string SubjectError
        {
            get => _subjectError;
            set => SetProperty(ref _subjectError, value);
        }
        
        public bool HasSubjectError => !string.IsNullOrEmpty(SubjectError);
        
        public string ClassError
        {
            get => _classError;
            set => SetProperty(ref _classError, value);
        }
        
        public bool HasClassError => !string.IsNullOrEmpty(ClassError);
        
        // List of possible assignment statuses
        public List<string> StatusOptions { get; } = new List<string> { "Draft", "Published", "Closed" };

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand PublishCommand { get; }

        // Constructor for creating a new assignment
        public AssignmentDialogViewModel(DatabaseService databaseService, Window dialogWindow, AuthenticationService authService)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _authService = authService;
            _isEditMode = false;
            
            // Initialize a new assignment
            _assignment = new Assignment
            {
                TeacherID = _authService.CurrentUser?.UserID ?? 0,
                CreatedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                MaxPoints = 100,
                Status = "Draft"
            };
            
            // Initialize commands
            SaveCommand = new RelayCommand(async _ => await SaveAssignmentAsync(), _ => CanSaveAssignment());
            CancelCommand = new RelayCommand(_ => CloseDialog(false));
            PublishCommand = new RelayCommand(async _ => await PublishAssignmentAsync(), _ => CanSaveAssignment());
            
            // Load subjects and classes
            LoadSubjectsAndClassesAsync();
        }

        // Constructor for editing an existing assignment
        public AssignmentDialogViewModel(DatabaseService databaseService, Window dialogWindow, AuthenticationService authService, Assignment assignment)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _authService = authService;
            _isEditMode = true;
            _originalAssignment = assignment;
            
            // Create a copy of the assignment to edit
            _assignment = new Assignment
            {
                AssignmentID = assignment.AssignmentID,
                Title = assignment.Title,
                Description = assignment.Description,
                CreatedDate = assignment.CreatedDate,
                DueDate = assignment.DueDate,
                MaxPoints = assignment.MaxPoints,
                TeacherID = assignment.TeacherID,
                SubjectID = assignment.SubjectID,
                SubjectName = assignment.SubjectName,
                ClassID = assignment.ClassID,
                ClassName = assignment.ClassName,
                Status = assignment.Status
            };
            
            // Initialize commands
            SaveCommand = new RelayCommand(async _ => await SaveAssignmentAsync(), _ => CanSaveAssignment());
            CancelCommand = new RelayCommand(_ => CloseDialog(false));
            PublishCommand = new RelayCommand(async _ => await PublishAssignmentAsync(), _ => CanSaveAssignment());
            
            // Load subjects and classes
            LoadSubjectsAndClassesAsync();
        }

        // Load available subjects and classes
        private async void LoadSubjectsAndClassesAsync()
        {
            try
            {
                IsProcessing = true;
                
                // Get teacher ID
                int teacherId = _authService.CurrentUser?.UserID ?? 0;
                if (teacherId == 0) return;
                
                // Load data concurrently
                await Task.WhenAll(
                    LoadSubjectsAsync(teacherId),
                    LoadClassesAsync(teacherId)
                );
                
                // If editing, set the selected subject and class
                if (_isEditMode)
                {
                    SelectedSubject = AvailableSubjects.FirstOrDefault(s => s.SubjectID == _assignment.SubjectID);
                    SelectedClass = AvailableClasses.FirstOrDefault(c => c.ClassID == _assignment.ClassID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        // Load subjects taught by this teacher
        private async Task LoadSubjectsAsync(int teacherId)
        {
            AvailableSubjects.Clear();
            
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
                AvailableSubjects.Add(new Subject
                {
                    SubjectID = Convert.ToInt32(row["SubjectID"]),
                    SubjectName = row["SubjectName"].ToString() ?? string.Empty
                });
            }
        }

        // Load classes taught by this teacher
        private async Task LoadClassesAsync(int teacherId)
        {
            AvailableClasses.Clear();
            
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
                AvailableClasses.Add(new Class
                {
                    ClassID = Convert.ToInt32(row["ClassID"]),
                    ClassName = row["ClassName"].ToString() ?? string.Empty
                });
            }
        }

        // Check if the assignment can be saved
        private bool CanSaveAssignment()
        {
            // Validate all fields
            ValidateTitle();
            ValidateDueDate();
            ValidateMaxPoints();
            ValidateSubject();
            ValidateClass();
            
            // Return true if there are no validation errors
            return !HasTitleError && !HasDueDateError && !HasMaxPointsError && !HasSubjectError && !HasClassError;
        }

        // Save the assignment to the database
        private async Task SaveAssignmentAsync()
        {
            try
            {
                if (!CanSaveAssignment()) return;
                
                IsProcessing = true;
                
                if (_isEditMode)
                {
                    // Update existing assignment
                    string query = @"
                        UPDATE Assignments
                        SET Title = @Title,
                            Description = @Description,
                            Deadline = @DueDate,
                            TotalMarks = @MaxPoints,
                            SubjectID = @SubjectID,
                            ClassID = @ClassID,
                            Status = @Status
                        WHERE AssignmentID = @AssignmentID";
                    
                    var parameters = new Dictionary<string, object>
                    {
                        { "@AssignmentID", _assignment.AssignmentID },
                        { "@Title", _assignment.Title },
                        { "@Description", _assignment.Description ?? string.Empty },
                        { "@DueDate", _assignment.DueDate },
                        { "@MaxPoints", _assignment.MaxPoints },
                        { "@SubjectID", _assignment.SubjectID },
                        { "@ClassID", _assignment.ClassID },
                        { "@Status", _assignment.Status }
                    };
                    
                    await _databaseService.ExecuteNonQueryAsync(query, parameters);
                    
                    MessageBox.Show("Assignment updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Insert new assignment
                    string query = @"
                        INSERT INTO Assignments 
                            (Title, Description, AssignedDate, Deadline, TotalMarks, TeacherID, SubjectID, ClassID, Status)
                        VALUES 
                            (@Title, @Description, @CreatedDate, @DueDate, @MaxPoints, @TeacherID, @SubjectID, @ClassID, @Status);
                        SELECT SCOPE_IDENTITY();";
                    
                    var parameters = new Dictionary<string, object>
                    {
                        { "@Title", _assignment.Title },
                        { "@Description", _assignment.Description ?? string.Empty },
                        { "@CreatedDate", _assignment.CreatedDate },
                        { "@DueDate", _assignment.DueDate },
                        { "@MaxPoints", _assignment.MaxPoints },
                        { "@TeacherID", _assignment.TeacherID },
                        { "@SubjectID", _assignment.SubjectID },
                        { "@ClassID", _assignment.ClassID },
                        { "@Status", _assignment.Status }
                    };
                    
                    var result = await _databaseService.ExecuteScalarAsync(query, parameters);
                    if (result != null)
                    {
                        _assignment.AssignmentID = Convert.ToInt32(result);
                    }
                    
                    MessageBox.Show("Assignment created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                CloseDialog(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving assignment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        // Save and publish the assignment
        private async Task PublishAssignmentAsync()
        {
            if (!CanSaveAssignment()) return;
            
            // Set the status to "Published" and save
            _assignment.Status = "Published";
            await SaveAssignmentAsync();
        }

        // Close the dialog
        private void CloseDialog(bool result)
        {
            _dialogWindow.DialogResult = result;
            _dialogWindow.Close();
        }

        // Validate the assignment title
        private void ValidateTitle()
        {
            if (string.IsNullOrWhiteSpace(_assignment.Title))
            {
                TitleError = "Title is required";
            }
            else if (_assignment.Title.Length < 3)
            {
                TitleError = "Title must be at least 3 characters";
            }
            else if (_assignment.Title.Length > 100)
            {
                TitleError = "Title must be less than 100 characters";
            }
            else
            {
                TitleError = string.Empty;
            }
        }

        // Validate the due date
        private void ValidateDueDate()
        {
            if (_assignment.DueDate < DateTime.Now)
            {
                DueDateError = "Due date must be in the future";
            }
            else
            {
                DueDateError = string.Empty;
            }
        }

        // Validate the max points
        private void ValidateMaxPoints()
        {
            if (_assignment.MaxPoints <= 0)
            {
                MaxPointsError = "Maximum points must be greater than 0";
            }
            else if (_assignment.MaxPoints > 1000)
            {
                MaxPointsError = "Maximum points must be less than or equal to 1000";
            }
            else
            {
                MaxPointsError = string.Empty;
            }
        }

        // Validate the subject selection
        private void ValidateSubject()
        {
            if (_selectedSubject == null || _selectedSubject.SubjectID <= 0)
            {
                SubjectError = "Please select a subject";
            }
            else
            {
                SubjectError = string.Empty;
            }
        }

        // Validate the class selection
        private void ValidateClass()
        {
            if (_selectedClass == null || _selectedClass.ClassID <= 0)
            {
                ClassError = "Please select a class";
            }
            else
            {
                ClassError = string.Empty;
            }
        }
    }
}
