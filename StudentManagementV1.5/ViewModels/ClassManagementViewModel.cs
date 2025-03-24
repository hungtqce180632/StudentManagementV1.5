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
    public class ClassManagementViewModel : ViewModelBase
    {
        private const string TeacherIdColumn = "TeacherID";
        private const string ClassIdColumn = "ClassID";
        private const string TeacherNameColumn = "TeacherName";
        
        private readonly DatabaseService _databaseService;
        private readonly NavigationService _navigationService;

        private ObservableCollection<Class> _classes = [];
        private string _searchText = string.Empty;
        private bool _showInactiveClasses = false;
        private bool _isLoading;
        private Class? _selectedClass;

        public ObservableCollection<Class> Classes
        {
            get => _classes;
            set => SetProperty(ref _classes, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = RefreshClassesAsync();
                }
            }
        }

        public bool ShowInactiveClasses
        {
            get => _showInactiveClasses;
            set
            {
                if (SetProperty(ref _showInactiveClasses, value))
                {
                    _ = RefreshClassesAsync();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public Class? SelectedClass
        {
            get => _selectedClass;
            set => SetProperty(ref _selectedClass, value);
        }

        public ICommand AddClassCommand { get; }
        public ICommand EditClassCommand { get; }
        public ICommand DeleteClassCommand { get; }
        public ICommand BackCommand { get; }

        public ClassManagementViewModel(DatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;

            AddClassCommand = new RelayCommand(_ => OpenAddClassDialog());
            EditClassCommand = new RelayCommand(param => OpenEditClassDialog(param as Class), param => param != null);
            DeleteClassCommand = new RelayCommand(async param => await DeleteClassAsync(param as Class), param => param != null);
            BackCommand = new RelayCommand(_ => _navigationService.NavigateTo(AppViews.AdminDashboard));
            
            // Load classes when ViewModel is created
            _ = LoadClassesAsync();
        }
        
        private async Task RefreshClassesAsync()
        {
            await LoadClassesAsync();
        }
        
        private async Task LoadClassesAsync()
        {
            try
            {
                IsLoading = true;
                Classes.Clear();

                string query = BuildClassQuery();
                var parameters = BuildQueryParameters();
                DataTable result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                PopulateClassesFromDataTable(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading classes: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private string BuildClassQuery()
        {
            string query = $"SELECT c.{ClassIdColumn}, c.ClassName, c.Grade, c.{TeacherIdColumn}, t.FirstName + ' ' + t.LastName AS {TeacherNameColumn}, " +
                          "c.ClassRoom, c.MaxCapacity, c.CurrentStudentCount, c.AcademicYear, c.IsActive " +
                          "FROM Classes c " +
                          $"LEFT JOIN Teachers t ON c.{TeacherIdColumn} = t.{TeacherIdColumn} " +
                          "WHERE 1=1";
            
            if (!_showInactiveClasses)
            {
                query += " AND c.IsActive = 1";
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                query += " AND (c.ClassName LIKE @Search OR c.Grade LIKE @Search OR c.ClassRoom LIKE @Search)";
            }

            query += " ORDER BY c.ClassName";
            
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
        
        private void PopulateClassesFromDataTable(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Classes.Add(new Class
                {
                    ClassID = Convert.ToInt32(row[ClassIdColumn]),
                    ClassName = row["ClassName"].ToString() ?? string.Empty,
                    Grade = row["Grade"].ToString() ?? string.Empty,
                    TeacherID = row[TeacherIdColumn] != DBNull.Value ? Convert.ToInt32(row[TeacherIdColumn]) : null,
                    TeacherName = row[TeacherNameColumn]?.ToString(),
                    ClassRoom = row["ClassRoom"].ToString() ?? string.Empty,
                    MaxCapacity = Convert.ToInt32(row["MaxCapacity"]),
                    CurrentStudentCount = Convert.ToInt32(row["CurrentStudentCount"]),
                    AcademicYear = row["AcademicYear"].ToString() ?? string.Empty,
                    IsActive = Convert.ToBoolean(row["IsActive"])
                });
            }
        }
        
        private void OpenAddClassDialog()
        {
            var dialog = new AddEditClassView();
            dialog.DataContext = new AddEditClassViewModel(_databaseService, dialog);
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true)
            {
                // Refresh the list after adding
                _ = RefreshClassesAsync();
            }
        }
        
        private void OpenEditClassDialog(Class? classObj)
        {
            if (classObj == null) return;
            
            var dialog = new AddEditClassView();
            dialog.DataContext = new AddEditClassViewModel(_databaseService, dialog, classObj);
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true)
            {
                // Refresh the list after editing
                _ = RefreshClassesAsync();
            }
        }
        
        private async Task DeleteClassAsync(Class? classObj)
        {
            if (classObj == null) return;
            
            // Confirm deletion
            var result = MessageBox.Show($"Are you sure you want to deactivate class {classObj.ClassName}?", 
                "Confirm Deactivate", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                await DeactivateClassAsync(classObj);
            }
        }
        
        private async Task DeactivateClassAsync(Class classObj)
        {
            try
            {
                IsLoading = true;
                
                // Check if class has students
                if (classObj.CurrentStudentCount > 0)
                {
                    MessageBox.Show("Cannot deactivate a class with active students. Please reassign the students first.",
                        "Operation Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Instead of deleting, set IsActive = 0
                string query = "UPDATE Classes SET IsActive = 0 WHERE ClassID = @ClassID";
                var parameters = new Dictionary<string, object>
                {
                    { "@ClassID", classObj.ClassID }
                };
                
                await _databaseService.ExecuteNonQueryAsync(query, parameters);
                
                // Refresh the list
                await LoadClassesAsync();
                
                MessageBox.Show($"Class {classObj.ClassName} has been deactivated.", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deactivating class: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
