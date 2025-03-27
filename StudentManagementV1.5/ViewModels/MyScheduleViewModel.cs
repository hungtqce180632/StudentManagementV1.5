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
    // Lớp MyScheduleViewModel
    // + Tại sao cần sử dụng: Quản lý và hiển thị lịch giảng dạy cá nhân của giáo viên
    // + Lớp này được gọi từ màn hình dashboard của giáo viên
    // + Chức năng chính: Hiển thị lịch học các lớp mà giáo viên đang dạy
    public class MyScheduleViewModel : ViewModelBase
    {
        // 1. Dịch vụ cơ sở dữ liệu để truy vấn lịch dạy
        // 2. Được truyền vào từ constructor
        // 3. Sử dụng để tải lịch dạy của giáo viên từ CSDL
        private readonly DatabaseService _databaseService;
        
        // 1. Dịch vụ điều hướng để quay lại dashboard
        // 2. Được truyền vào từ constructor
        // 3. Sử dụng để điều hướng giữa các màn hình
        private readonly NavigationService _navigationService;
        
        // 1. Dịch vụ xác thực để lấy thông tin giáo viên đăng nhập
        // 2. Được truyền vào từ constructor
        // 3. Sử dụng để xác định ID của giáo viên hiện tại
        private readonly AuthenticationService _authService;
        
        // 1. Danh sách lịch dạy trong tuần
        // 2. Binding đến DataGrid trong giao diện
        // 3. Chứa thông tin lịch dạy của giáo viên
        private ObservableCollection<Schedule> _schedules = new ObservableCollection<Schedule>();
        public ObservableCollection<Schedule> Schedules
        {
            get => _schedules;
            set => SetProperty(ref _schedules, value);
        }
        
        // 1. Ngày được chọn để lọc lịch dạy
        // 2. Binding đến ComboBox chọn ngày
        // 3. Sử dụng để lọc lịch dạy theo ngày
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
        
        // 1. Danh sách các ngày trong tuần
        // 2. Binding đến ComboBox chọn ngày
        // 3. Bao gồm tùy chọn "Tất cả các ngày" và các ngày trong tuần
        private ObservableCollection<string> _days = new ObservableCollection<string>();
        public ObservableCollection<string> Days
        {
            get => _days;
            set => SetProperty(ref _days, value);
        }
        
        // 1. Cờ đánh dấu đang tải dữ liệu
        // 2. Binding đến loading indicator
        // 3. Hiển thị trạng thái đang tải
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        
        // 1. Thông báo lỗi hoặc trạng thái
        // 2. Binding đến TextBlock hiển thị lỗi
        // 3. Hiển thị khi không có lịch dạy hoặc có lỗi xảy ra
        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        
        // 1. Lệnh quay lại dashboard
        // 2. Binding đến nút "Quay lại"
        // 3. Điều hướng về màn hình dashboard của giáo viên
        public ICommand BackCommand { get; }
        
        // 1. Lệnh áp dụng bộ lọc
        // 2. Binding đến nút "Áp dụng"
        // 3. Tải lại lịch dạy với ngày đã chọn
        public ICommand ApplyFilterCommand { get; }
        
        // 1. Constructor của ViewModel
        // 2. Nhận các dịch vụ cần thiết
        // 3. Khởi tạo lệnh và tải dữ liệu ban đầu
        public MyScheduleViewModel(DatabaseService databaseService, NavigationService navigationService, AuthenticationService authService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _authService = authService;
            
            // Khởi tạo lệnh
            BackCommand = new RelayCommand(_ => _navigationService.NavigateTo(AppViews.TeacherDashboard));
            ApplyFilterCommand = new RelayCommand(_ => LoadSchedulesAsync());
            
            // Khởi tạo danh sách ngày
            InitializeDaysFilter();
            
            // Tải lịch dạy
            LoadSchedulesAsync();
        }
        
        // 1. Phương thức khởi tạo bộ lọc ngày
        // 2. Thêm tùy chọn "Tất cả các ngày" và các ngày trong tuần
        // 3. Thiết lập giá trị mặc định là "Tất cả các ngày"
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
        
        // 1. Phương thức tải lịch dạy của giáo viên
        // 2. Truy vấn CSDL để lấy lịch dạy với bộ lọc
        // 3. Cập nhật danh sách lịch dạy để hiển thị
        private async void LoadSchedulesAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                Schedules.Clear();
                
                // Lấy ID của giáo viên đăng nhập
                int teacherId = _authService.CurrentUser?.UserID ?? 0;
                if (teacherId == 0)
                {
                    ErrorMessage = "Không tìm thấy thông tin giáo viên. Vui lòng đăng nhập lại.";
                    return;
                }
                
                // Tìm TeacherID từ UserID
                string teacherQuery = "SELECT TeacherID FROM Teachers WHERE UserID = @UserID";
                var teacherParams = new Dictionary<string, object> { { "@UserID", teacherId } };
                var teacherResult = await _databaseService.ExecuteScalarAsync(teacherQuery, teacherParams);
                
                if (teacherResult == null)
                {
                    ErrorMessage = "Không tìm thấy thông tin giáo viên. Vui lòng đăng nhập lại.";
                    return;
                }
                
                int actualTeacherId = Convert.ToInt32(teacherResult);
                
                // Truy vấn lịch dạy
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
                    WHERE cs.TeacherID = @TeacherID";
                
                var parameters = new Dictionary<string, object>
                {
                    { "@TeacherID", actualTeacherId }
                };
                
                // Thêm bộ lọc ngày nếu không phải "Tất cả các ngày"
                if (SelectedDay != "All Days")
                {
                    query += " AND cs.DayOfWeek = @DayOfWeek";
                    parameters.Add("@DayOfWeek", SelectedDay);
                }
                
                // Sắp xếp theo thứ tự ngày trong tuần và thời gian bắt đầu
                query += " ORDER BY CASE cs.DayOfWeek " +
                         "WHEN 'Monday' THEN 1 " +
                         "WHEN 'Tuesday' THEN 2 " +
                         "WHEN 'Wednesday' THEN 3 " +
                         "WHEN 'Thursday' THEN 4 " +
                         "WHEN 'Friday' THEN 5 " +
                         "WHEN 'Saturday' THEN 6 " +
                         "WHEN 'Sunday' THEN 7 END, cs.StartTime";
                
                // Thực hiện truy vấn
                var result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                // Xử lý kết quả
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
                
                // Hiển thị thông báo nếu không có lịch dạy
                if (Schedules.Count == 0)
                {
                    ErrorMessage = "Không tìm thấy lịch dạy nào.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi khi tải lịch dạy: {ex.Message}";
                MessageBox.Show(ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
