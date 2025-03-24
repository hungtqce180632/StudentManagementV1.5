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
    public class AddEditClassViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly Window _dialogWindow;
        private bool _isEditMode;
        private string _dialogTitle;
        private string _errorMessage = string.Empty;
        private Class _class;
        private ObservableCollection<TeacherViewModel> _teachers = new ObservableCollection<TeacherViewModel>();
        private TeacherViewModel _selectedTeacher;

        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetProperty(ref _dialogTitle, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public Class Class
        {
            get => _class;
            set => SetProperty(ref _class, value);
        }

        public ObservableCollection<TeacherViewModel> Teachers
        {
            get => _teachers;
            set => SetProperty(ref _teachers, value);
        }

        public TeacherViewModel SelectedTeacher
        {
            get => _selectedTeacher;
            set
            {
                if (SetProperty(ref _selectedTeacher, value) && value != null)
                {
                    Class.TeacherID = value.TeacherID;
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Constructor for adding a new class
        public AddEditClassViewModel(DatabaseService databaseService, Window dialogWindow)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _isEditMode = false;
            DialogTitle = "Add New Class";

            Class = new Class
            {
                IsActive = true,
                MaxCapacity = 30,
                CurrentStudentCount = 0,
                AcademicYear = DateTime.Now.Year + "-" + (DateTime.Now.Year + 1)
            };

            SaveCommand = new RelayCommand(async param => await SaveClass(), param => CanSaveClass());
            CancelCommand = new RelayCommand(param => CloseDialog());

            LoadTeachersAsync();
        }

        // Constructor for editing an existing class
        public AddEditClassViewModel(DatabaseService databaseService, Window dialogWindow, Class classToEdit)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _isEditMode = true;
            DialogTitle = "Edit Class";

            // Create a copy of the class to edit
            Class = new Class
            {
                ClassID = classToEdit.ClassID,
                ClassName = classToEdit.ClassName,
                Grade = classToEdit.Grade,
                TeacherID = classToEdit.TeacherID,
                ClassRoom = classToEdit.ClassRoom,
                MaxCapacity = classToEdit.MaxCapacity,
                CurrentStudentCount = classToEdit.CurrentStudentCount,
                AcademicYear = classToEdit.AcademicYear,
                IsActive = classToEdit.IsActive
            };

            SaveCommand = new RelayCommand(async param => await SaveClass(), param => CanSaveClass());
            CancelCommand = new RelayCommand(param => CloseDialog());

            LoadTeachersAsync();
        }

        private async void LoadTeachersAsync()
        {
            try
            {
                // Add "No Teacher" option
                Teachers.Add(new TeacherViewModel { TeacherID = null, FullName = "-- No Teacher --" });

                // Load teachers from database
                string query = "SELECT TeacherID, FirstName, LastName FROM Teachers ORDER BY LastName, FirstName";
                var result = await _databaseService.ExecuteQueryAsync(query);

                foreach (DataRow row in result.Rows)
                {
                    Teachers.Add(new TeacherViewModel
                    {
                        TeacherID = Convert.ToInt32(row["TeacherID"]),
                        FullName = $"{row["FirstName"]} {row["LastName"]}"
                    });
                }

                // Set selected teacher if class has a teacher
                if (Class.TeacherID.HasValue)
                {
                    foreach (var teacher in Teachers)
                    {
                        if (teacher.TeacherID == Class.TeacherID)
                        {
                            SelectedTeacher = teacher;
                            break;
                        }
                    }
                }
                else
                {
                    // Default to "No Teacher"
                    SelectedTeacher = Teachers[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teachers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveClass()
        {
            return !string.IsNullOrWhiteSpace(Class.ClassName) && 
                   !string.IsNullOrWhiteSpace(Class.AcademicYear);
        }

        private async Task SaveClass()
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate input
                if (string.IsNullOrWhiteSpace(Class.ClassName))
                {
                    ErrorMessage = "Class name is required.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Class.AcademicYear))
                {
                    ErrorMessage = "Academic year is required.";
                    return;
                }

                // Set TeacherID based on selection
                Class.TeacherID = SelectedTeacher?.TeacherID;

                if (_isEditMode)
                {
                    // Update existing class
                    string query = @"
                        UPDATE Classes 
                        SET ClassName = @ClassName, 
                            Grade = @Grade, 
                            TeacherID = @TeacherID, 
                            ClassRoom = @ClassRoom, 
                            MaxCapacity = @MaxCapacity, 
                            AcademicYear = @AcademicYear, 
                            IsActive = @IsActive 
                        WHERE ClassID = @ClassID";

                    var parameters = new Dictionary<string, object>
                    {
                        { "@ClassID", Class.ClassID },
                        { "@ClassName", Class.ClassName },
                        { "@Grade", Class.Grade ?? (object)DBNull.Value },
                        { "@TeacherID", Class.TeacherID.HasValue ? (object)Class.TeacherID : DBNull.Value },
                        { "@ClassRoom", Class.ClassRoom ?? (object)DBNull.Value },
                        { "@MaxCapacity", Class.MaxCapacity },
                        { "@AcademicYear", Class.AcademicYear },
                        { "@IsActive", Class.IsActive }
                    };

                    await _databaseService.ExecuteNonQueryAsync(query, parameters);
                    MessageBox.Show("Class updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Add new class
                    string query = @"
                        INSERT INTO Classes (ClassName, Grade, TeacherID, ClassRoom, MaxCapacity, CurrentStudentCount, AcademicYear, IsActive)
                        VALUES (@ClassName, @Grade, @TeacherID, @ClassRoom, @MaxCapacity, @CurrentStudentCount, @AcademicYear, @IsActive);
                        SELECT SCOPE_IDENTITY();";

                    var parameters = new Dictionary<string, object>
                    {
                        { "@ClassName", Class.ClassName },
                        { "@Grade", Class.Grade ?? (object)DBNull.Value },
                        { "@TeacherID", Class.TeacherID.HasValue ? (object)Class.TeacherID : DBNull.Value },
                        { "@ClassRoom", Class.ClassRoom ?? (object)DBNull.Value },
                        { "@MaxCapacity", Class.MaxCapacity },
                        { "@CurrentStudentCount", 0 },
                        { "@AcademicYear", Class.AcademicYear },
                        { "@IsActive", Class.IsActive }
                    };

                    var result = await _databaseService.ExecuteScalarAsync(query, parameters);
                    if (result != null)
                    {
                        Class.ClassID = Convert.ToInt32(result);
                        MessageBox.Show("Class added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                // Close the dialog
                _dialogWindow.DialogResult = true;
                _dialogWindow.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving class: {ex.Message}";
                MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseDialog()
        {
            _dialogWindow.DialogResult = false;
            _dialogWindow.Close();
        }
    }

    // Helper class for teacher selection
    public class TeacherViewModel
    {
        public int? TeacherID { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
