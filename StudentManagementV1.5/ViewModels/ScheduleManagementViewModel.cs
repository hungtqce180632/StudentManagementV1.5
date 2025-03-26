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
    // Lớp ScheduleManagementViewModel
    // + Tại sao cần sử dụng: Quản lý và hiển thị lịch học, thời khóa biểu trong hệ thống
    // + Lớp này được gọi từ màn hình quản lý lịch học
    // + Chức năng chính: Xem, lọc, thêm, sửa và xóa lịch học cho các lớp
    public class ScheduleManagementViewModel : ViewModelBase
    {
        // 1. Dịch vụ cơ sở dữ liệu để truy vấn và cập nhật lịch học
        // 2. Được truyền vào từ constructor
        // 3. Dùng để tải và thao tác với dữ liệu lịch học
        private readonly DatabaseService _databaseService;
        
        // 1. Dịch vụ điều hướng
        // 2. Được truyền vào từ constructor
        // 3. Sử dụng để điều hướng giữa các màn hình
        private readonly NavigationService _navigationService;

        // 1. Danh sách lịch học hiển thị trong UI
        // 2. Binding đến DataGrid hoặc ListView
        // 3. Được tải từ cơ sở dữ liệu và lọc theo các điều kiện
        private ObservableCollection<Schedule> _schedules = new ObservableCollection<Schedule>();
        public ObservableCollection<Schedule> Schedules
        {
            get => _schedules;
            set => SetProperty(ref _schedules, value);
        }
        
        // 1. Danh sách lớp học để lọc
        // 2. Binding đến ComboBox lớp học
        // 3. Bao gồm tùy chọn "Tất cả lớp học" và các lớp học từ cơ sở dữ liệu
        private ObservableCollection<SchoolClass> _classes = new ObservableCollection<SchoolClass>();
        public ObservableCollection<SchoolClass> Classes
        {
            get => _classes;
            set => SetProperty(ref _classes, value);
        }
        
        // 1. Lớp học được chọn để lọc
        // 2. Binding đến ComboBox lớp học
        // 3. Dùng để lọc lịch học theo lớp
        private SchoolClass _selectedClass;
        public SchoolClass SelectedClass
        {
            get => _selectedClass;
            set
            {
                if (SetProperty(ref _selectedClass, value))
                {
                    LoadSchedulesAsync();
                }
            }
        }
        
        // 1. Danh sách các ngày trong tuần để lọc
        // 2. Binding đến ComboBox ngày trong tuần
        // 3. Bao gồm tùy chọn "Tất cả các ngày" và các ngày trong tuần
        private ObservableCollection<string> _days = new ObservableCollection<string>();
        public ObservableCollection<string> Days
        {
            get => _days;
            set => SetProperty(ref _days, value);
        }
        
        // 1. Ngày trong tuần được chọn để lọc
        // 2. Binding đến ComboBox ngày trong tuần
        // 3. Dùng để lọc lịch học theo ngày
        private string _selectedDay = "All Days";
        public string SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (SetProperty(ref _selectedDay, value))
                {
                    LoadSchedulesAsync();
                }
            }
        }
        
        // 1. Cờ đánh dấu đang tải dữ liệu
        // 2. Binding đến overlay loading
        // 3. Hiển thị hiệu ứng loading khi đang tải dữ liệu
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        
        // 1. Lịch học được chọn trong danh sách
        // 2. Binding đến SelectedItem của DataGrid
        // 3. Được sử dụng cho các thao tác sửa và xóa
        private Schedule _selectedSchedule;
        public Schedule SelectedSchedule
        {
            get => _selectedSchedule;
            set => SetProperty(ref _selectedSchedule, value);
        }
        
        // 1. Lệnh áp dụng bộ lọc
        // 2. Binding đến nút "Áp dụng" trong UI
        // 3. Khi được gọi, tải lại lịch học với các bộ lọc đã chọn
        public ICommand ApplyFiltersCommand { get; }
        
        // 1. Lệnh thêm lịch học mới
        // 2. Binding đến nút "Thêm lịch" trong UI
        // 3. Khi được gọi, mở hộp thoại thêm lịch học
        public ICommand AddScheduleCommand { get; }
        
        // 1. Lệnh chỉnh sửa lịch học
        // 2. Binding đến nút "Sửa" trong UI
        // 3. Khi được gọi, mở hộp thoại sửa lịch học đã chọn
        public ICommand EditScheduleCommand { get; }
        
        // 1. Lệnh xóa lịch học
        // 2. Binding đến nút "Xóa" trong UI
        // 3. Khi được gọi, hiển thị xác nhận và xóa lịch học đã chọn
        public ICommand DeleteScheduleCommand { get; }
        
        // 1. Lệnh quay lại màn hình trước
        // 2. Binding đến nút "Quay lại" trong UI
        // 3. Khi được gọi, chuyển về màn hình dashboard
        public ICommand NavigateBackCommand { get; }
        
        // 1. Constructor của lớp
        // 2. Khởi tạo các tham số và thiết lập lệnh
        // 3. Tải dữ liệu ban đầu từ cơ sở dữ liệu
        public ScheduleManagementViewModel(DatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;

            // Initialize commands
            ApplyFiltersCommand = new RelayCommand(param => LoadSchedulesAsync());
            AddScheduleCommand = new RelayCommand(param => AddSchedule());
            EditScheduleCommand = new RelayCommand(param => EditSchedule(param as Schedule), param => param != null);
            DeleteScheduleCommand = new RelayCommand(async param => await DeleteScheduleAsync(param as Schedule), param => param != null);
            NavigateBackCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.AdminDashboard));

            // Initialize days of week filter
            InitializeDaysFilter();

            // Load initial data
            LoadClassesAsync();
            LoadSchedulesAsync();
        }
        
        // 1. Phương thức khởi tạo bộ lọc ngày trong tuần
        // 2. Thêm tùy chọn "Tất cả các ngày" và các ngày trong tuần
        // 3. Thiết lập lựa chọn mặc định là "Tất cả các ngày"
        private void InitializeDaysFilter()
        {
            Days.Clear();
            Days.Add("All Days");
            Days.Add("Monday");
            Days.Add("Tuesday");
            Days.Add("Wednesday");
            Days.Add("Thursday");
            Days.Add("Friday");
            Days.Add("Saturday");
            Days.Add("Sunday");
            
            SelectedDay = Days[0]; // Default to "All Days"
        }

        // 1. Phương thức tải danh sách lớp học
        // 2. Thêm tùy chọn "Tất cả lớp học" và tải danh sách từ cơ sở dữ liệu
        // 3. Cập nhật danh sách Classes và thiết lập lớp được chọn mặc định
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

        // 1. Phương thức tải danh sách lịch học
        // 2. Tạo truy vấn dựa trên các bộ lọc đã chọn
        // 3. Cập nhật danh sách Schedules với kết quả từ cơ sở dữ liệu
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

                // Apply class filter
                if (SelectedClass != null && SelectedClass.ClassID > 0)
                {
                    query += " AND cs.ClassID = @ClassID";
                    parameters.Add("@ClassID", SelectedClass.ClassID);
                }

                // Apply day filter
                if (SelectedDay != "All Days")
                {
                    query += " AND cs.DayOfWeek = @DayOfWeek";
                    parameters.Add("@DayOfWeek", SelectedDay);
                }

                // Order by
                query += " ORDER BY c.ClassName, cs.DayOfWeek, cs.StartTime";

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

        // 1. Phương thức thêm lịch học mới
        // 2. Mở dialog thêm lịch học và xử lý kết quả
        // 3. Nếu lưu thành công, tải lại danh sách lịch học
        private void AddSchedule()
        {
            var dialogWindow = new AddEditScheduleView();
            var viewModel = new AddEditScheduleViewModel(_databaseService, dialogWindow);
            dialogWindow.DataContext = viewModel;
            dialogWindow.Owner = Application.Current.MainWindow;
            
            bool? result = dialogWindow.ShowDialog();
            if (result == true)
            {
                LoadSchedulesAsync();
            }
        }

        // 1. Phương thức chỉnh sửa lịch học
        // 2. Mở dialog chỉnh sửa lịch học với dữ liệu đã chọn
        // 3. Nếu lưu thành công, tải lại danh sách lịch học
        private void EditSchedule(Schedule schedule)
        {
            if (schedule == null) return;
            
            var dialogWindow = new AddEditScheduleView();
            var viewModel = new AddEditScheduleViewModel(_databaseService, dialogWindow, schedule);
            dialogWindow.DataContext = viewModel;
            dialogWindow.Owner = Application.Current.MainWindow;
            
            bool? result = dialogWindow.ShowDialog();
            if (result == true)
            {
                LoadSchedulesAsync();
            }
        }

        // 1. Phương thức xóa lịch học
        // 2. Hiển thị xác nhận xóa và thực hiện xóa nếu được xác nhận
        // 3. Tải lại danh sách lịch học sau khi xóa thành công
        private async Task DeleteScheduleAsync(Schedule schedule)
        {
            if (schedule == null) return;
            
            var result = MessageBox.Show($"Are you sure you want to delete the schedule for {schedule.ClassName} - {schedule.SubjectName}?", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    IsLoading = true;
                    
                    string query = "DELETE FROM ClassSchedules WHERE ScheduleID = @ScheduleID";
                    var parameters = new Dictionary<string, object>
                    {
                        { "@ScheduleID", schedule.ScheduleID }
                    };
                    
                    await _databaseService.ExecuteNonQueryAsync(query, parameters);
                    
                    // Reload schedules after deletion
                    LoadSchedulesAsync();
                    
                    MessageBox.Show("Schedule deleted successfully.", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting schedule: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }
    }
}
