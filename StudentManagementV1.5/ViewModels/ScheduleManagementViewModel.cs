using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    public class ScheduleManagementViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly NavigationService _navigationService;

        private ObservableCollection<Schedule> _schedules = new ObservableCollection<Schedule>();
        private ObservableCollection<SchoolClass> _classes = new ObservableCollection<SchoolClass>();
        private ObservableCollection<string> _daysOfWeek = new ObservableCollection<string>
        {
            "All", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"
        };

        private SchoolClass _selectedClass;
        private string _selectedDay = "All";
        private Schedule _selectedSchedule;
        private bool _isLoading;

        // Properties
        public ObservableCollection<Schedule> Schedules
        {
            get => _schedules;
            set => SetProperty(ref _schedules, value);
        }

        public ObservableCollection<SchoolClass> Classes
        {
            get => _classes;
            set => SetProperty(ref _classes, value);
        }

        public ObservableCollection<string> DaysOfWeek
        {
            get => _daysOfWeek;
            set => SetProperty(ref _daysOfWeek, value);
        }

        public SchoolClass SelectedClass
        {
            get => _selectedClass;
            set => SetProperty(ref _selectedClass, value);
        }

        public string SelectedDay
        {
            get => _selectedDay;
            set => SetProperty(ref _selectedDay, value);
        }

        public Schedule SelectedSchedule
        {
            get => _selectedSchedule;
            set => SetProperty(ref _selectedSchedule, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Commands
        public ICommand ApplyFiltersCommand { get; }
        public ICommand AddScheduleCommand { get; }
        public ICommand EditScheduleCommand { get; }
        public ICommand DeleteScheduleCommand { get; }
        public ICommand NavigateBackCommand { get; }

        // Constructor
        public ScheduleManagementViewModel(DatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;

            // Initialize commands
            ApplyFiltersCommand = new RelayCommand(param => LoadSchedulesAsync());
            AddScheduleCommand = new RelayCommand(param => AddSchedule());
            EditScheduleCommand = new RelayCommand(param => EditSchedule(param as Schedule), param => param != null);
            DeleteScheduleCommand = new RelayCommand(param => DeleteSchedule(param as Schedule), param => param != null);
            NavigateBackCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.AdminDashboard));

            // Load initial data
            LoadClassesAsync();
            LoadSchedulesAsync();
        }

        // Methods
        private async void LoadClassesAsync()
        {
            try
            {
                IsLoading = true;
                Classes.Clear();

                // First add an "All Classes" item
                Classes.Add(new SchoolClass { ClassID = 0, ClassName = "All Classes" });

                // Load classes from database
                string query = "SELECT ClassID, ClassName, AcademicYear, IsActive FROM Classes WHERE IsActive = 1 ORDER BY ClassName";
                var result = await _databaseService.ExecuteQueryAsync(query);

                foreach (DataRow row in result.Rows)
                {
                    Classes.Add(new SchoolClass
                    {
                        ClassID = Convert.ToInt32(row["ClassID"]),
                        ClassName = row["ClassName"].ToString(),
                        AcademicYear = row["AcademicYear"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"])
                    });
                }

                // Default selection
                SelectedClass = Classes[0]; // "All Classes"
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

        private async void LoadSchedulesAsync()
        {
            try
            {
                IsLoading = true;
                Schedules.Clear();

                string query = @"
                    SELECT 
                        cs.ScheduleID, 
                        c.ClassID, c.ClassName, 
                        s.SubjectID, s.SubjectName, 
                        t.TeacherID, t.FirstName + ' ' + t.LastName AS TeacherName,
                        cs.DayOfWeek, cs.StartTime, cs.EndTime, cs.Room
                    FROM ClassSchedules cs
                    JOIN Classes c ON cs.ClassID = c.ClassID
                    JOIN Subjects s ON cs.SubjectID = s.SubjectID
                    JOIN Teachers t ON cs.TeacherID = t.TeacherID
                    WHERE 1=1";

                var parameters = new Dictionary<string, object>();

                if (SelectedClass != null && SelectedClass.ClassID != 0)
                {
                    query += " AND cs.ClassID = @ClassID";
                    parameters["@ClassID"] = SelectedClass.ClassID;
                }

                if (SelectedDay != "All")
                {
                    query += " AND cs.DayOfWeek = @DayOfWeek";
                    parameters["@DayOfWeek"] = SelectedDay;
                }

                query += @"
                    ORDER BY CASE cs.DayOfWeek
                        WHEN 'Monday' THEN 1
                        WHEN 'Tuesday' THEN 2
                        WHEN 'Wednesday' THEN 3
                        WHEN 'Thursday' THEN 4
                        WHEN 'Friday' THEN 5
                        WHEN 'Saturday' THEN 6
                        WHEN 'Sunday' THEN 7 END,
                    cs.StartTime";

                DataTable result = await _databaseService.ExecuteQueryAsync(query, parameters);

                foreach (DataRow row in result.Rows)
                {
                    Schedules.Add(new Schedule
                    {
                        ScheduleID = Convert.ToInt32(row["ScheduleID"]),
                        ClassID = Convert.ToInt32(row["ClassID"]),
                        ClassName = row["ClassName"].ToString(),
                        SubjectID = Convert.ToInt32(row["SubjectID"]),
                        SubjectName = row["SubjectName"].ToString(),
                        TeacherID = Convert.ToInt32(row["TeacherID"]),
                        TeacherName = row["TeacherName"].ToString(),
                        DayOfWeek = row["DayOfWeek"].ToString(),
                        StartTime = (TimeSpan)row["StartTime"],
                        EndTime = (TimeSpan)row["EndTime"],
                        Room = row["Room"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedules: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddSchedule()
        {
            MessageBox.Show("Add Schedule functionality will be implemented soon.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditSchedule(Schedule schedule)
        {
            if (schedule == null) return;
            
            MessageBox.Show($"Edit Schedule functionality for {schedule.ClassName} - {schedule.SubjectName} will be implemented soon.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteSchedule(Schedule schedule)
        {
            if (schedule == null) return;
            
            var result = MessageBox.Show($"Are you sure you want to delete the schedule for {schedule.ClassName} - {schedule.SubjectName}?", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    IsLoading = true;
                    
                    // For now, just show a message - we'll implement the actual deletion in a future update
                    MessageBox.Show("Delete Schedule functionality will be implemented soon.", 
                        "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                    // In a real implementation, we would delete from the database and refresh the list
                    // Sample code (commented out):
                    // string query = $"DELETE FROM ClassSchedules WHERE ScheduleID = {schedule.ScheduleID}";
                    // await _databaseService.ExecuteNonQueryAsync(query);
                    // LoadSchedulesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting schedule: {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }
    }
}
