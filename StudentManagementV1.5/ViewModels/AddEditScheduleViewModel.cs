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
    // Lớp AddEditScheduleViewModel
    // + Tại sao cần sử dụng: Xử lý logic cho việc thêm mới hoặc chỉnh sửa lịch học
    // + Lớp này được gọi từ ScheduleManagementViewModel khi mở dialog thêm/sửa lịch học
    // + Chức năng chính: Xác thực dữ liệu, xử lý lưu và cập nhật thông tin lịch học
    public class AddEditScheduleViewModel : ViewModelBase
    {
        // 1. Dịch vụ cơ sở dữ liệu để truy vấn và cập nhật thông tin lịch học
        // 2. Được truyền vào từ constructor
        // 3. Dùng để tải và thao tác với dữ liệu lịch học
        private readonly DatabaseService _databaseService;
        
        // 1. Cửa sổ dialog hiện tại
        // 2. Được truyền vào từ constructor
        // 3. Dùng để đóng dialog khi hoàn thành
        private readonly Window _dialogWindow;
        
        // 1. Lịch học hiện tại đang được chỉnh sửa
        // 2. Null nếu đang thêm mới lịch học
        // 3. Được truyền vào từ ScheduleManagementViewModel
        private Schedule _currentSchedule;
        
        // 1. Sự kiện yêu cầu đóng dialog
        // 2. Được gọi khi người dùng lưu thành công hoặc hủy
        // 3. Truyền kết quả về cho View thông qua DialogResult
        public event Action<bool> RequestClose;
        
        // 1. Cờ đánh dấu đây là chế độ chỉnh sửa hay thêm mới
        // 2. True nếu đang chỉnh sửa lịch học hiện có
        // 3. False nếu đang thêm mới lịch học
        private bool _isEditMode;
        
        // 1. Tiêu đề của dialog
        // 2. Thay đổi dựa trên việc thêm mới hay chỉnh sửa
        // 3. Hiển thị trên thanh tiêu đề của dialog
        private string _dialogTitle = "Add New Schedule";
        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetProperty(ref _dialogTitle, value);
        }
        
        // 1. Trạng thái xử lý của dialog
        // 2. True khi đang xử lý tác vụ bất đồng bộ (như kiểm tra hoặc lưu)
        // 3. Dùng để hiển thị giao diện loading
        private bool _isProcessing;
        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }
        
        // 1. Danh sách lớp học có sẵn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Được tải từ cơ sở dữ liệu
        private ObservableCollection<SchoolClass> _classes = new ObservableCollection<SchoolClass>();
        public ObservableCollection<SchoolClass> Classes
        {
            get => _classes;
            set => SetProperty(ref _classes, value);
        }
        
        // 1. Lớp học được chọn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Dùng để lấy ClassID khi lưu lịch học
        private SchoolClass _selectedClass;
        public SchoolClass SelectedClass
        {
            get => _selectedClass;
            set
            {
                if (SetProperty(ref _selectedClass, value))
                {
                    ValidateClass();
                }
            }
        }
        
        // 1. Lỗi lớp học
        // 2. Hiển thị khi chưa chọn lớp học
        // 3. Rỗng khi không có lỗi
        private string _classError = string.Empty;
        public string ClassError
        {
            get => _classError;
            set => SetProperty(ref _classError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi lớp học hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasClassError => !string.IsNullOrEmpty(ClassError);
        
        // 1. Danh sách môn học có sẵn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Được tải từ cơ sở dữ liệu
        private ObservableCollection<SubjectItem> _subjects = new ObservableCollection<SubjectItem>();
        public ObservableCollection<SubjectItem> Subjects
        {
            get => _subjects;
            set => SetProperty(ref _subjects, value);
        }
        
        // 1. Môn học được chọn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Dùng để lấy SubjectID khi lưu lịch học
        private SubjectItem _selectedSubject;
        public SubjectItem SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                if (SetProperty(ref _selectedSubject, value))
                {
                    ValidateSubject();
                }
            }
        }
        
        // 1. Lỗi môn học
        // 2. Hiển thị khi chưa chọn môn học
        // 3. Rỗng khi không có lỗi
        private string _subjectError = string.Empty;
        public string SubjectError
        {
            get => _subjectError;
            set => SetProperty(ref _subjectError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi môn học hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasSubjectError => !string.IsNullOrEmpty(SubjectError);
        
        // 1. Danh sách giáo viên có sẵn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Được tải từ cơ sở dữ liệu
        private ObservableCollection<Teacher> _teachers = new ObservableCollection<Teacher>();
        public ObservableCollection<Teacher> Teachers
        {
            get => _teachers;
            set => SetProperty(ref _teachers, value);
        }
        
        // 1. Giáo viên được chọn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Dùng để lấy TeacherID khi lưu lịch học
        private Teacher _selectedTeacher;
        public Teacher SelectedTeacher
        {
            get => _selectedTeacher;
            set
            {
                if (SetProperty(ref _selectedTeacher, value))
                {
                    ValidateTeacher();
                }
            }
        }
        
        // 1. Lỗi giáo viên
        // 2. Hiển thị khi chưa chọn giáo viên
        // 3. Rỗng khi không có lỗi
        private string _teacherError = string.Empty;
        public string TeacherError
        {
            get => _teacherError;
            set => SetProperty(ref _teacherError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi giáo viên hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasTeacherError => !string.IsNullOrEmpty(TeacherError);
        
        // 1. Danh sách các ngày trong tuần
        // 2. Binding đến ComboBox trong giao diện
        // 3. Cố định và không thay đổi
        public ObservableCollection<string> DaysOfWeek { get; } = new ObservableCollection<string>
        {
            "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"
        };
        
        // 1. Ngày trong tuần được chọn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Dùng để lưu vào trường DayOfWeek trong bảng ClassSchedules
        private string _selectedDayOfWeek;
        public string SelectedDayOfWeek
        {
            get => _selectedDayOfWeek;
            set
            {
                if (SetProperty(ref _selectedDayOfWeek, value))
                {
                    ValidateDayOfWeek();
                }
            }
        }
        
        // 1. Lỗi ngày trong tuần
        // 2. Hiển thị khi chưa chọn ngày trong tuần
        // 3. Rỗng khi không có lỗi
        private string _dayOfWeekError = string.Empty;
        public string DayOfWeekError
        {
            get => _dayOfWeekError;
            set => SetProperty(ref _dayOfWeekError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi ngày trong tuần hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasDayOfWeekError => !string.IsNullOrEmpty(DayOfWeekError);
        
        // 1. Danh sách các khung giờ có sẵn
        // 2. Binding đến ComboBox thời gian bắt đầu và kết thúc
        // 3. Được tạo với khoảng cách 30 phút
        public ObservableCollection<string> TimeSlots { get; } = new ObservableCollection<string>();
        
        // 1. Thời gian bắt đầu được chọn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Dùng để lưu vào trường StartTime trong bảng ClassSchedules
        private string _selectedStartTime;
        public string SelectedStartTime
        {
            get => _selectedStartTime;
            set
            {
                if (SetProperty(ref _selectedStartTime, value))
                {
                    ValidateStartTime();
                    ValidateEndTime(); // Re-validate end time when start time changes
                }
            }
        }
        
        // 1. Lỗi thời gian bắt đầu
        // 2. Hiển thị khi chưa chọn thời gian bắt đầu
        // 3. Rỗng khi không có lỗi
        private string _startTimeError = string.Empty;
        public string StartTimeError
        {
            get => _startTimeError;
            set => SetProperty(ref _startTimeError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi thời gian bắt đầu hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasStartTimeError => !string.IsNullOrEmpty(StartTimeError);
        
        // 1. Thời gian kết thúc được chọn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Dùng để lưu vào trường EndTime trong bảng ClassSchedules
        private string _selectedEndTime;
        public string SelectedEndTime
        {
            get => _selectedEndTime;
            set
            {
                if (SetProperty(ref _selectedEndTime, value))
                {
                    ValidateEndTime();
                }
            }
        }
        
        // 1. Lỗi thời gian kết thúc
        // 2. Hiển thị khi chưa chọn thời gian kết thúc hoặc kết thúc <= bắt đầu
        // 3. Rỗng khi không có lỗi
        private string _endTimeError = string.Empty;
        public string EndTimeError
        {
            get => _endTimeError;
            set => SetProperty(ref _endTimeError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi thời gian kết thúc hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasEndTimeError => !string.IsNullOrEmpty(EndTimeError);
        
        // 1. Phòng học
        // 2. Binding đến TextBox trong giao diện
        // 3. Dùng để lưu vào trường Room trong bảng ClassSchedules
        private string _room = string.Empty;
        public string Room
        {
            get => _room;
            set
            {
                if (SetProperty(ref _room, value))
                {
                    ValidateRoom();
                }
            }
        }
        
        // 1. Lỗi phòng học
        // 2. Hiển thị khi chưa nhập phòng học
        // 3. Rỗng khi không có lỗi
        private string _roomError = string.Empty;
        public string RoomError
        {
            get => _roomError;
            set => SetProperty(ref _roomError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi phòng học hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasRoomError => !string.IsNullOrEmpty(RoomError);
        
        // 1. Lệnh lưu lịch học
        // 2. Binding đến nút "Lưu" trong giao diện
        // 3. Thực hiện xác thực và lưu dữ liệu
        public ICommand SaveCommand { get; }
        
        // 1. Lệnh hủy thao tác
        // 2. Binding đến nút "Hủy" trong giao diện
        // 3. Đóng dialog mà không lưu
        public ICommand CancelCommand { get; }

        // 1. Constructor cho lịch học mới
        // 2. Khởi tạo ViewModel để thêm lịch học mới
        // 3. Thiết lập tiêu đề và trạng thái phù hợp
        public AddEditScheduleViewModel(DatabaseService databaseService, Window dialogWindow)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _isEditMode = false;
            DialogTitle = "Add New Schedule";
            
            InitializeTimeSlots();
            
            SaveCommand = new RelayCommand(async param => await SaveScheduleAsync(), param => CanSaveSchedule());
            CancelCommand = new RelayCommand(param => CloseDialog(false));
            
            // Load data
            LoadClassesAsync();
            LoadSubjectsAsync();
            LoadTeachersAsync();
        }
        
        // 1. Constructor cho chỉnh sửa lịch học
        // 2. Khởi tạo ViewModel với thông tin lịch học hiện có
        // 3. Điền dữ liệu từ lịch học được chỉnh sửa
        public AddEditScheduleViewModel(DatabaseService databaseService, Window dialogWindow, Schedule schedule)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _currentSchedule = schedule;
            _isEditMode = true;
            DialogTitle = "Edit Schedule";
            
            InitializeTimeSlots();
            
            SaveCommand = new RelayCommand(async param => await SaveScheduleAsync(), param => CanSaveSchedule());
            CancelCommand = new RelayCommand(param => CloseDialog(false));
            
            // Load data
            LoadClassesAsync();
            LoadSubjectsAsync();
            LoadTeachersAsync();
            
            // Set initial values after loading data
            Room = schedule.Room;
            SelectedDayOfWeek = schedule.DayOfWeek;
            SelectedStartTime = schedule.StartTime.ToString(@"hh\:mm");
            SelectedEndTime = schedule.EndTime.ToString(@"hh\:mm");
        }
        
        // 1. Phương thức khởi tạo danh sách khung giờ
        // 2. Tạo danh sách các khung giờ từ 7:00 đến 22:00 với khoảng cách 30 phút
        // 3. Thêm các khung giờ vào danh sách TimeSlots
        private void InitializeTimeSlots()
        {
            TimeSlots.Clear();
            
            TimeSpan start = new TimeSpan(7, 0, 0); // Start at 7:00 AM
            TimeSpan end = new TimeSpan(22, 0, 0);  // End at 10:00 PM
            TimeSpan interval = new TimeSpan(0, 30, 0); // 30-minute intervals
            
            for (TimeSpan time = start; time <= end; time = time.Add(interval))
            {
                TimeSlots.Add(time.ToString(@"hh\:mm"));
            }
        }
        
        // 1. Phương thức tải danh sách lớp học
        // 2. Tải các lớp học đang hoạt động từ cơ sở dữ liệu
        // 3. Cập nhật danh sách Classes và thiết lập lớp được chọn
        private async void LoadClassesAsync()
        {
            try
            {
                IsProcessing = true;
                Classes.Clear();

                string query = "SELECT ClassID, ClassName, AcademicYear FROM Classes WHERE IsActive = 1 ORDER BY ClassName";
                var result = await _databaseService.ExecuteQueryAsync(query);

                foreach (DataRow row in result.Rows)
                {
                    Classes.Add(new SchoolClass
                    {
                        ClassID = Convert.ToInt32(row["ClassID"]),
                        ClassName = row["ClassName"].ToString() ?? string.Empty,
                        AcademicYear = row["AcademicYear"].ToString() ?? string.Empty
                    });
                }

                // Set selected class if in edit mode
                if (_isEditMode && _currentSchedule != null)
                {
                    SelectedClass = Classes.FirstOrDefault(c => c.ClassID == _currentSchedule.ClassID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading classes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }
        
        // 1. Phương thức tải danh sách môn học
        // 2. Tải các môn học đang hoạt động từ cơ sở dữ liệu
        // 3. Cập nhật danh sách Subjects và thiết lập môn học được chọn
        private async void LoadSubjectsAsync()
        {
            try
            {
                IsProcessing = true;
                Subjects.Clear();

                string query = "SELECT SubjectID, SubjectName FROM Subjects WHERE IsActive = 1 ORDER BY SubjectName";
                var result = await _databaseService.ExecuteQueryAsync(query);

                foreach (DataRow row in result.Rows)
                {
                    Subjects.Add(new SubjectItem
                    {
                        SubjectID = Convert.ToInt32(row["SubjectID"]),
                        SubjectName = row["SubjectName"].ToString() ?? string.Empty
                    });
                }

                // Set selected subject if in edit mode
                if (_isEditMode && _currentSchedule != null)
                {
                    SelectedSubject = Subjects.FirstOrDefault(s => s.SubjectID == _currentSchedule.SubjectID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }
        
        // 1. Phương thức tải danh sách giáo viên
        // 2. Tải danh sách giáo viên từ cơ sở dữ liệu
        // 3. Cập nhật danh sách Teachers và thiết lập giáo viên được chọn
        private async void LoadTeachersAsync()
        {
            try
            {
                IsProcessing = true;
                Teachers.Clear();

                string query = @"
                    SELECT t.TeacherID, t.FirstName, t.LastName, u.IsActive 
                    FROM Teachers t
                    JOIN Users u ON t.UserID = u.UserID
                    WHERE u.IsActive = 1
                    ORDER BY t.LastName, t.FirstName";
                var result = await _databaseService.ExecuteQueryAsync(query);

                foreach (DataRow row in result.Rows)
                {
                    string firstName = row["FirstName"].ToString() ?? string.Empty;
                    string lastName = row["LastName"].ToString() ?? string.Empty;
                    
                    Teachers.Add(new Teacher
                    {
                        TeacherID = Convert.ToInt32(row["TeacherID"]),
                        TeacherName = $"{firstName} {lastName}".Trim()
                    });
                }

                // Set selected teacher if in edit mode
                if (_isEditMode && _currentSchedule != null)
                {
                    SelectedTeacher = Teachers.FirstOrDefault(t => t.TeacherID == _currentSchedule.TeacherID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teachers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }
        
        // 1. Phương thức xác thực lớp học
        // 2. Kiểm tra xem đã chọn lớp học chưa
        // 3. Cập nhật lỗi nếu chưa chọn lớp học
        private void ValidateClass()
        {
            ClassError = SelectedClass == null ? "Please select a class" : string.Empty;
            OnPropertyChanged(nameof(HasClassError));
        }
        
        // 1. Phương thức xác thực môn học
        // 2. Kiểm tra xem đã chọn môn học chưa
        // 3. Cập nhật lỗi nếu chưa chọn môn học
        private void ValidateSubject()
        {
            SubjectError = SelectedSubject == null ? "Please select a subject" : string.Empty;
            OnPropertyChanged(nameof(HasSubjectError));
        }
        
        // 1. Phương thức xác thực giáo viên
        // 2. Kiểm tra xem đã chọn giáo viên chưa
        // 3. Cập nhật lỗi nếu chưa chọn giáo viên
        private void ValidateTeacher()
        {
            TeacherError = SelectedTeacher == null ? "Please select a teacher" : string.Empty;
            OnPropertyChanged(nameof(HasTeacherError));
        }
        
        // 1. Phương thức xác thực ngày trong tuần
        // 2. Kiểm tra xem đã chọn ngày trong tuần chưa
        // 3. Cập nhật lỗi nếu chưa chọn ngày trong tuần
        private void ValidateDayOfWeek()
        {
            DayOfWeekError = string.IsNullOrEmpty(SelectedDayOfWeek) ? "Please select a day of week" : string.Empty;
            OnPropertyChanged(nameof(HasDayOfWeekError));
        }
        
        // 1. Phương thức xác thực thời gian bắt đầu
        // 2. Kiểm tra xem đã chọn thời gian bắt đầu chưa
        // 3. Cập nhật lỗi nếu chưa chọn thời gian bắt đầu
        private void ValidateStartTime()
        {
            StartTimeError = string.IsNullOrEmpty(SelectedStartTime) ? "Please select a start time" : string.Empty;
            OnPropertyChanged(nameof(HasStartTimeError));
        }
        
        // 1. Phương thức xác thực thời gian kết thúc
        // 2. Kiểm tra xem đã chọn thời gian kết thúc chưa và thời gian kết thúc > bắt đầu
        // 3. Cập nhật lỗi nếu không hợp lệ
        private void ValidateEndTime()
        {
            EndTimeError = string.Empty;
            
            if (string.IsNullOrEmpty(SelectedEndTime))
            {
                EndTimeError = "Please select an end time";
            }
            else if (!string.IsNullOrEmpty(SelectedStartTime))
            {
                // Check if end time is after start time
                TimeSpan startTime, endTime;
                if (TimeSpan.TryParse(SelectedStartTime, out startTime) && 
                    TimeSpan.TryParse(SelectedEndTime, out endTime))
                {
                    if (endTime <= startTime)
                    {
                        EndTimeError = "End time must be after start time";
                    }
                }
            }
            
            OnPropertyChanged(nameof(HasEndTimeError));
        }
        
        // 1. Phương thức xác thực phòng học
        // 2. Kiểm tra xem đã nhập phòng học chưa
        // 3. Cập nhật lỗi nếu chưa nhập phòng học
        private void ValidateRoom()
        {
            RoomError = string.IsNullOrWhiteSpace(Room) ? "Please enter a room" : string.Empty;
            OnPropertyChanged(nameof(HasRoomError));
        }
        
        // 1. Phương thức kiểm tra xem có thể lưu lịch học không
        // 2. Kiểm tra tất cả các trường bắt buộc đã được nhập
        // 3. Trả về true nếu tất cả các trường hợp lệ, false nếu không
        private bool CanSaveSchedule()
        {
            bool isValid = SelectedClass != null &&
                          SelectedSubject != null &&
                          SelectedTeacher != null &&
                          !string.IsNullOrEmpty(SelectedDayOfWeek) &&
                          !string.IsNullOrEmpty(SelectedStartTime) &&
                          !string.IsNullOrEmpty(SelectedEndTime) &&
                          !string.IsNullOrWhiteSpace(Room) &&
                          !HasClassError &&
                          !HasSubjectError &&
                          !HasTeacherError &&
                          !HasDayOfWeekError &&
                          !HasStartTimeError &&
                          !HasEndTimeError &&
                          !HasRoomError;
            
            return isValid;
        }
        
        // 1. Phương thức kiểm tra xung đột lịch học
        // 2. Kiểm tra xem lịch học mới có trùng với lịch học đã tồn tại không
        // 3. Trả về true nếu có xung đột, false nếu không
        private async Task<bool> CheckScheduleConflictAsync(int scheduleId = 0)
        {
            if (SelectedClass == null || SelectedTeacher == null || 
                string.IsNullOrEmpty(SelectedDayOfWeek) || 
                string.IsNullOrEmpty(SelectedStartTime) || 
                string.IsNullOrEmpty(SelectedEndTime))
                return false;
            
            TimeSpan startTime = TimeSpan.Parse(SelectedStartTime);
            TimeSpan endTime = TimeSpan.Parse(SelectedEndTime);
            
            string query = @"
                SELECT COUNT(*) FROM ClassSchedules
                WHERE (
                    (ClassID = @ClassID AND DayOfWeek = @DayOfWeek AND 
                    ((StartTime <= @StartTime AND EndTime > @StartTime) OR
                     (StartTime < @EndTime AND EndTime >= @EndTime) OR
                     (StartTime >= @StartTime AND EndTime <= @EndTime)))
                    OR
                    (TeacherID = @TeacherID AND DayOfWeek = @DayOfWeek AND 
                    ((StartTime <= @StartTime AND EndTime > @StartTime) OR
                     (StartTime < @EndTime AND EndTime >= @EndTime) OR
                     (StartTime >= @StartTime AND EndTime <= @EndTime)))
                )";
            
            // If editing, exclude the current schedule from the check
            if (scheduleId > 0)
            {
                query += " AND ScheduleID != @ScheduleID";
            }
            
            var parameters = new Dictionary<string, object>
            {
                { "@ClassID", SelectedClass.ClassID },
                { "@TeacherID", SelectedTeacher.TeacherID },
                { "@DayOfWeek", SelectedDayOfWeek },
                { "@StartTime", startTime },
                { "@EndTime", endTime }
            };
            
            if (scheduleId > 0)
            {
                parameters.Add("@ScheduleID", scheduleId);
            }
            
            int count = Convert.ToInt32(await _databaseService.ExecuteScalarAsync(query, parameters));
            return count > 0;
        }
        
        // 1. Phương thức lưu lịch học
        // 2. Xác thực dữ liệu và kiểm tra xung đột trước khi lưu
        // 3. Thêm mới hoặc cập nhật lịch học tùy thuộc vào _isEditMode
        private async Task SaveScheduleAsync()
        {
            // Validate all fields
            ValidateClass();
            ValidateSubject();
            ValidateTeacher();
            ValidateDayOfWeek();
            ValidateStartTime();
            ValidateEndTime();
            ValidateRoom();
            
            if (!CanSaveSchedule()) return;
            
            IsProcessing = true;
            
            try
            {
                // Check for schedule conflicts
                bool hasConflict = await CheckScheduleConflictAsync(_isEditMode ? _currentSchedule.ScheduleID : 0);
                
                if (hasConflict)
                {
                    MessageBox.Show("There is a schedule conflict with an existing class or teacher schedule.",
                        "Schedule Conflict", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                TimeSpan startTime = TimeSpan.Parse(SelectedStartTime);
                TimeSpan endTime = TimeSpan.Parse(SelectedEndTime);
                
                if (_isEditMode)
                {
                    // Update existing schedule
                    string query = @"
                        UPDATE ClassSchedules
                        SET ClassID = @ClassID,
                            SubjectID = @SubjectID,
                            TeacherID = @TeacherID,
                            DayOfWeek = @DayOfWeek,
                            StartTime = @StartTime,
                            EndTime = @EndTime,
                            Room = @Room
                        WHERE ScheduleID = @ScheduleID";
                    
                    var parameters = new Dictionary<string, object>
                    {
                        { "@ScheduleID", _currentSchedule.ScheduleID },
                        { "@ClassID", SelectedClass.ClassID },
                        { "@SubjectID", SelectedSubject.SubjectID },
                        { "@TeacherID", SelectedTeacher.TeacherID },
                        { "@DayOfWeek", SelectedDayOfWeek },
                        { "@StartTime", startTime },
                        { "@EndTime", endTime },
                        { "@Room", Room }
                    };
                    
                    await _databaseService.ExecuteNonQueryAsync(query, parameters);
                }
                else
                {
                    // Insert new schedule
                    string query = @"
                        INSERT INTO ClassSchedules (ClassID, SubjectID, TeacherID, DayOfWeek, StartTime, EndTime, Room)
                        VALUES (@ClassID, @SubjectID, @TeacherID, @DayOfWeek, @StartTime, @EndTime, @Room)";
                    
                    var parameters = new Dictionary<string, object>
                    {
                        { "@ClassID", SelectedClass.ClassID },
                        { "@SubjectID", SelectedSubject.SubjectID },
                        { "@TeacherID", SelectedTeacher.TeacherID },
                        { "@DayOfWeek", SelectedDayOfWeek },
                        { "@StartTime", startTime },
                        { "@EndTime", endTime },
                        { "@Room", Room }
                    };
                    
                    await _databaseService.ExecuteNonQueryAsync(query, parameters);
                }
                
                CloseDialog(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving schedule: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }
        
        // 1. Phương thức đóng dialog
        // 2. Gọi sự kiện RequestClose để thông báo kết quả
        // 3. true nếu lưu thành công, false nếu hủy
        private void CloseDialog(bool result)
        {
            if (RequestClose != null)
            {
                RequestClose.Invoke(result);
            }
            else
            {
                _dialogWindow.DialogResult = result;
                _dialogWindow.Close();
            }
        }
    }
    
    // Lớp SubjectItem đơn giản hóa để sử dụng trong ComboBox
    // + Tại sao cần sử dụng: Lưu trữ thông tin cơ bản của môn học để hiển thị trong ComboBox
    // + Lớp này chỉ chứa các thuộc tính cần thiết cho việc hiển thị và xử lý trong AddEditScheduleViewModel
    // + Chức năng chính: Cung cấp cấu trúc dữ liệu đơn giản cho đối tượng môn học
    public class SubjectItem
    {
        // 1. ID của môn học
        // 2. Dùng để liên kết với bảng Subjects
        // 3. Được sử dụng khi lưu lịch học
        public int SubjectID { get; set; }
        
        // 1. Tên môn học
        // 2. Hiển thị trong ComboBox
        // 3. Dùng để người dùng dễ dàng chọn môn học
        public string SubjectName { get; set; } = string.Empty;
    }
    
    // Lớp Teacher đơn giản hóa để sử dụng trong ComboBox
    // + Tại sao cần sử dụng: Lưu trữ thông tin cơ bản của giáo viên để hiển thị trong ComboBox
    // + Lớp này chỉ chứa các thuộc tính cần thiết cho việc hiển thị và xử lý trong AddEditScheduleViewModel
    // + Chức năng chính: Cung cấp cấu trúc dữ liệu đơn giản cho đối tượng giáo viên
    public class Teacher
    {
        // 1. ID của giáo viên
        // 2. Dùng để liên kết với bảng Teachers
        // 3. Được sử dụng khi lưu lịch học
        public int TeacherID { get; set; }
        
        // 1. Tên đầy đủ của giáo viên
        // 2. Hiển thị trong ComboBox
        // 3. Dùng để người dùng dễ dàng chọn giáo viên
        public string TeacherName { get; set; } = string.Empty;
    }
    
    // Lớp SchoolClass đơn giản hóa để sử dụng trong ComboBox
    // + Tại sao cần sử dụng: Lưu trữ thông tin cơ bản của lớp học để hiển thị trong ComboBox
    // + Lớp này chỉ chứa các thuộc tính cần thiết cho việc hiển thị và xử lý trong AddEditScheduleViewModel
    // + Chức năng chính: Cung cấp cấu trúc dữ liệu đơn giản cho đối tượng lớp học
    public class SchoolClass
    {
        // 1. ID của lớp học
        // 2. Dùng để liên kết với bảng Classes
        // 3. Được sử dụng khi lưu lịch học
        public int ClassID { get; set; }
        
        // 1. Tên lớp học
        // 2. Hiển thị trong ComboBox
        // 3. Dùng để người dùng dễ dàng chọn lớp học
        public string ClassName { get; set; } = string.Empty;
        
        // 1. Năm học của lớp
        // 2. Thông tin bổ sung về lớp học
        // 3. Không hiển thị trong ComboBox nhưng có thể được sử dụng nếu cần
        public string AcademicYear { get; set; } = string.Empty;
        
        // 1. Trạng thái của lớp học
        // 2. Xác định lớp học có đang hoạt động hay không
        // 3. Không hiển thị trong ComboBox nhưng có thể được sử dụng nếu cần
        public bool IsActive { get; set; } = true;
    }
}
