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
        
        // 1. Danh sách lớp học để lọc lịch học
        // 2. Binding đến ComboBox lọc theo lớp
        // 3. Được tải từ cơ sở dữ liệu
        private ObservableCollection<SchoolClass> _classes = new ObservableCollection<SchoolClass>();
        
        // 1. Danh sách các ngày trong tuần để lọc
        // 2. Binding đến ComboBox lọc theo ngày
        // 3. Danh sách cố định các ngày trong tuần
        private ObservableCollection<string> _daysOfWeek = new ObservableCollection<string>
        {
            "All", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"
        };

        // 1. Lớp học được chọn để lọc
        // 2. Binding đến SelectedItem của ComboBox lớp học
        // 3. Được sử dụng khi áp dụng bộ lọc
        private SchoolClass _selectedClass;
        
        // 1. Ngày trong tuần được chọn để lọc
        // 2. Binding đến SelectedItem của ComboBox ngày
        // 3. Được sử dụng khi áp dụng bộ lọc
        private string _selectedDay = "All";
        
        // 1. Lịch học được chọn trong danh sách
        // 2. Binding đến SelectedItem của DataGrid/ListView
        // 3. Dùng cho các thao tác sửa, xóa hoặc hiển thị chi tiết
        private Schedule _selectedSchedule;
        
        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến UI để hiển thị thông báo "Đang tải..."
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        private bool _isLoading;

        // 1. Danh sách lịch học hiển thị trong UI
        // 2. Binding đến DataGrid hoặc ListView
        // 3. Được tải từ cơ sở dữ liệu và lọc theo các điều kiện
        public ObservableCollection<Schedule> Schedules
        {
            get => _schedules;
            set => SetProperty(ref _schedules, value);
        }

        // 1. Danh sách lớp học để lọc lịch học
        // 2. Binding đến ComboBox lọc theo lớp
        // 3. Được tải từ cơ sở dữ liệu
        public ObservableCollection<SchoolClass> Classes
        {
            get => _classes;
            set => SetProperty(ref _classes, value);
        }

        // 1. Danh sách các ngày trong tuần để lọc
        // 2. Binding đến ComboBox lọc theo ngày
        // 3. Danh sách cố định các ngày trong tuần
        public ObservableCollection<string> DaysOfWeek
        {
            get => _daysOfWeek;
            set => SetProperty(ref _daysOfWeek, value);
        }

        // 1. Lớp học được chọn để lọc
        // 2. Binding đến SelectedItem của ComboBox lớp học
        // 3. Được sử dụng khi áp dụng bộ lọc
        public SchoolClass SelectedClass
        {
            get => _selectedClass;
            set => SetProperty(ref _selectedClass, value);
        }

        // 1. Ngày trong tuần được chọn để lọc
        // 2. Binding đến SelectedItem của ComboBox ngày
        // 3. Được sử dụng khi áp dụng bộ lọc
        public string SelectedDay
        {
            get => _selectedDay;
            set => SetProperty(ref _selectedDay, value);
        }

        // 1. Lịch học được chọn trong danh sách
        // 2. Binding đến SelectedItem của DataGrid/ListView
        // 3. Dùng cho các thao tác sửa, xóa hoặc hiển thị chi tiết
        public Schedule SelectedSchedule
        {
            get => _selectedSchedule;
            set => SetProperty(ref _selectedSchedule, value);
        }

        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến UI để hiển thị thông báo "Đang tải..."
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
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
            DeleteScheduleCommand = new RelayCommand(param => DeleteSchedule(param as Schedule), param => param != null);
            NavigateBackCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.AdminDashboard));

            // Load initial data
            LoadClassesAsync();
            LoadSchedulesAsync();
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

        // 1. Phương thức thêm lịch học mới
        // 2. Hiển thị thông báo chức năng sẽ được triển khai trong tương lai
        // 3. Đặt chỗ cho chức năng thêm lịch học thực sự
        private void AddSchedule()
        {
            MessageBox.Show("Add Schedule functionality will be implemented soon.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 1. Phương thức chỉnh sửa lịch học
        // 2. Hiển thị thông báo chức năng sẽ được triển khai trong tương lai
        // 3. Đặt chỗ cho chức năng sửa lịch học thực sự
        private void EditSchedule(Schedule schedule)
        {
            if (schedule == null) return;
            
            MessageBox.Show($"Edit Schedule functionality for {schedule.ClassName} - {schedule.SubjectName} will be implemented soon.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 1. Phương thức xóa lịch học
        // 2. Hiển thị xác nhận xóa và thông báo chức năng sẽ được triển khai trong tương lai
        // 3. Đặt chỗ cho chức năng xóa lịch học thực sự
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
