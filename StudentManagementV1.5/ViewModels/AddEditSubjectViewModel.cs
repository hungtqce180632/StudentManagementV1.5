using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
using System.Windows.Input;
using System.Windows;

namespace StudentManagementV1._5.ViewModels
{
    public class AddEditSubjectViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly Window _dialogWindow;
        private bool _isEditMode;
        private Subject _subject;

        public string DialogTitle => _isEditMode ? "Edit Subject" : "Add New Subject";

        public Subject Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        public string ErrorMessage { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditSubjectViewModel(DatabaseService databaseService, Window dialogWindow)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _isEditMode = false; // Default is add new subject

            Subject = new Subject();

            SaveCommand = new RelayCommand(async param => await SaveSubject(), param => CanSaveSubject());
            CancelCommand = new RelayCommand(param => CloseDialog());
        }

        public AddEditSubjectViewModel(DatabaseService databaseService, Window dialogWindow, Subject subjectToEdit)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _isEditMode = true;
            Subject = subjectToEdit;

            SaveCommand = new RelayCommand(async param => await SaveSubject(), param => CanSaveSubject());
            CancelCommand = new RelayCommand(param => CloseDialog());
        }

        private bool CanSaveSubject()
        {
            return !string.IsNullOrWhiteSpace(Subject.SubjectName); // Make sure SubjectName is not empty
        }

        private async Task SaveSubject()
        {
            try
            {
                if (_isEditMode)
                {
                    // Update the existing subject in the database
                    string query = "UPDATE Subjects SET SubjectName = @SubjectName, Description = @Description, Credits = @Credits, IsActive = @IsActive WHERE SubjectID = @SubjectID";
                    var parameters = new Dictionary<string, object>
                {
                    { "@SubjectID", Subject.SubjectID },
                    { "@SubjectName", Subject.SubjectName },
                    { "@Description", Subject.Description },
                    { "@Credits", Subject.Credits },
                    { "@IsActive", Subject.IsActive }
                };

                    await _databaseService.ExecuteNonQueryAsync(query, parameters);
                }
                else
                {
                    // Add a new subject to the database
                    string query = "INSERT INTO Subjects (SubjectName, Description, Credits, IsActive) VALUES (@SubjectName, @Description, @Credits, @IsActive)";
                    var parameters = new Dictionary<string, object>
                {
                    { "@SubjectName", Subject.SubjectName },
                    { "@Description", Subject.Description },
                    { "@Credits", Subject.Credits },
                    { "@IsActive", Subject.IsActive }
                };

                    await _databaseService.ExecuteNonQueryAsync(query, parameters);
                }

                // Close the dialog if the subject is successfully saved
                _dialogWindow.DialogResult = true;
                _dialogWindow.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving subject: {ex.Message}";
            }
        }

        private void CloseDialog()
        {
            _dialogWindow.DialogResult = false;
            _dialogWindow.Close();
        }
    }
}
