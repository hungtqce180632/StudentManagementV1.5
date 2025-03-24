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
    public class SubjectManagementViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly NavigationService _navigationService;

        private ObservableCollection<Subject> _subjects = [];
        private string _searchText = string.Empty;
        private bool _showInactiveSubjects = false;
        private bool _isLoading;
        private Subject? _selectedSubject;

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
                    _ = RefreshSubjectsAsync();
                }
            }
        }

        public bool ShowInactiveSubjects
        {
            get => _showInactiveSubjects;
            set
            {
                if (SetProperty(ref _showInactiveSubjects, value))
                {
                    _ = RefreshSubjectsAsync();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set => SetProperty(ref _selectedSubject, value);
        }

        public ICommand AddSubjectCommand { get; }
        public ICommand EditSubjectCommand { get; }
        public ICommand DeleteSubjectCommand { get; }
        public ICommand BackCommand { get; }

        public SubjectManagementViewModel(DatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;

            AddSubjectCommand = new RelayCommand(_ => AddSubject());
            EditSubjectCommand = new RelayCommand(param => EditSubject(param as Subject), param => param != null);
            DeleteSubjectCommand = new RelayCommand(async param => await DeleteSubjectAsync(param as Subject), param => param != null);
            BackCommand = new RelayCommand(_ => _navigationService.NavigateTo(AppViews.AdminDashboard));
            
            // Load subjects when ViewModel is created
            _ = LoadSubjectsAsync();
        }
        
        private async Task RefreshSubjectsAsync()
        {
            await LoadSubjectsAsync();
        }
        
        private async Task LoadSubjectsAsync()
        {
            try
            {
                IsLoading = true;
                Subjects.Clear();

                string query = BuildSubjectQuery();
                var parameters = BuildQueryParameters();
                DataTable result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                PopulateSubjectsFromDataTable(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private string BuildSubjectQuery()
        {
            string query = @"
                SELECT 
                    s.SubjectID, 
                    s.SubjectName, 
                    s.Description, 
                    s.Credits, 
                    s.IsActive,
                    (SELECT COUNT(DISTINCT ClassID) FROM TeacherSubjects WHERE SubjectID = s.SubjectID) AS ClassCount,
                    (SELECT COUNT(DISTINCT TeacherID) FROM TeacherSubjects WHERE SubjectID = s.SubjectID) AS TeacherCount
                FROM Subjects s
                WHERE 1=1";
            
            if (!_showInactiveSubjects)
            {
                query += " AND s.IsActive = 1";
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                query += " AND (s.SubjectName LIKE @Search OR s.Description LIKE @Search)";
            }

            query += " ORDER BY s.SubjectName";
            
            return query;
        }
        
        private Dictionary<string, object> BuildQueryParameters()
        {
            var parameters = new Dictionary<string, object>();
            
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                parameters["@Search"] = $"%{_searchText}%";
            }
            
            return parameters;
        }
        
        private void PopulateSubjectsFromDataTable(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Subjects.Add(new Subject
                {
                    SubjectID = Convert.ToInt32(row["SubjectID"]),
                    SubjectName = row["SubjectName"].ToString() ?? string.Empty,
                    Description = row["Description"].ToString() ?? string.Empty,
                    Credits = row["Credits"] != DBNull.Value ? Convert.ToInt32(row["Credits"]) : null,
                    ClassCount = row["ClassCount"] != DBNull.Value ? Convert.ToInt32(row["ClassCount"]) : 0,
                    TeacherCount = row["TeacherCount"] != DBNull.Value ? Convert.ToInt32(row["TeacherCount"]) : 0,
                    IsActive = Convert.ToBoolean(row["IsActive"])
                });
            }
        }
        
        private void AddSubject()
        {
            // For now just show a placeholder message
            // In a future update, implement an Add Subject dialog similar to Class Management
            MessageBox.Show("Add Subject functionality will be implemented in a future update.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void EditSubject(Subject? subject)
        {
            if (subject == null) return;
            
            // For now just show a placeholder message
            // In a future update, implement an Edit Subject dialog similar to Class Management
            MessageBox.Show($"Edit Subject '{subject.SubjectName}' functionality will be implemented in a future update.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private async Task DeleteSubjectAsync(Subject? subject)
        {
            if (subject == null) return;
            
            // Confirm deletion
            var result = MessageBox.Show($"Are you sure you want to deactivate subject '{subject.SubjectName}'?", 
                "Confirm Deactivate", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                await DeactivateSubjectAsync(subject);
            }
        }
        
        private async Task DeactivateSubjectAsync(Subject subject)
        {
            try
            {
                IsLoading = true;
                
                // Check if subject is in use
                if (subject.ClassCount > 0 || subject.TeacherCount > 0)
                {
                    MessageBox.Show("Cannot deactivate a subject that is currently being used in classes or by teachers. " +
                                    "Please reassign all classes and teachers first.",
                        "Operation Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Instead of deleting, set IsActive = 0
                string query = "UPDATE Subjects SET IsActive = 0 WHERE SubjectID = @SubjectID";
                var parameters = new Dictionary<string, object>
                {
                    { "@SubjectID", subject.SubjectID }
                };
                
                await _databaseService.ExecuteNonQueryAsync(query, parameters);
                
                // Refresh the list
                await LoadSubjectsAsync();

                MessageBox.Show($"Subject '{subject.SubjectName}' has been deactivated.", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deactivating subject: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
