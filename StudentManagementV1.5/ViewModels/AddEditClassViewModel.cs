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
    // Lớp AddEditClassViewModel
    // + Tại sao cần sử dụng: Cung cấp chức năng tạo mới và chỉnh sửa lớp học
    // + Lớp này được gọi từ ClassManagementViewModel khi cần tạo hoặc sửa lớp học
    // + Chức năng chính: Quản lý giao diện, xác thực dữ liệu và lưu lớp học
    public class AddEditClassViewModel : ViewModelBase
    {
        // 1. Dịch vụ cơ sở dữ liệu để truy vấn và cập nhật lớp học
        // 2. Được truyền vào từ constructor
        // 3. Dùng để lưu lớp học và tải danh sách giáo viên
        private readonly DatabaseService _databaseService;
        
        // 1. Cửa sổ hộp thoại đang hiển thị
        // 2. Được truyền vào từ constructor
        // 3. Dùng để đóng hộp thoại khi hoàn tất
        private readonly Window _dialogWindow;
        
        // 1. Chế độ hiện tại: thêm mới hoặc chỉnh sửa
        // 2. Được thiết lập trong constructor
        // 3. Ảnh hưởng đến giao diện và hành vi của form
        private bool _isEditMode;
        
        // 1. Tiêu đề của hộp thoại
        // 2. Hiển thị ở phần trên của hộp thoại
        // 3. Thay đổi theo chế độ: "Add New Class" hoặc "Edit Class"
        private string _dialogTitle;
        
        // 1. Thông báo lỗi khi xác thực dữ liệu
        // 2. Hiển thị khi dữ liệu không hợp lệ
        // 3. Được xóa khi dữ liệu đã được sửa
        private string _errorMessage = string.Empty;
        
        // 1. Đối tượng lớp học đang được chỉnh sửa hoặc tạo mới
        // 2. Chứa tất cả thông tin về lớp học
        // 3. Được binding đến các control trong form
        private Class _class;
        
        // 1. Danh sách giáo viên có thể chọn làm chủ nhiệm
        // 2. Được tải từ cơ sở dữ liệu
        // 3. Hiển thị trong ComboBox để người dùng chọn
        private ObservableCollection<TeacherViewModel> _teachers = new ObservableCollection<TeacherViewModel>();
        
        // 1. Giáo viên được chọn làm chủ nhiệm
        // 2. Được cập nhật khi người dùng chọn từ ComboBox
        // 3. Cập nhật thuộc tính TeacherID của lớp học
        private TeacherViewModel _selectedTeacher;

        // 1. Tiêu đề của hộp thoại
        // 2. Binding đến phần Header của Window
        // 3. Dựa trên chế độ thêm mới hoặc chỉnh sửa
        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetProperty(ref _dialogTitle, value);
        }

        // 1. Thông báo lỗi khi xác thực
        // 2. Binding đến TextBlock hiển thị lỗi
        // 3. Hiển thị các lỗi khi người dùng nhập dữ liệu không hợp lệ
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // 1. Đối tượng lớp học đang được chỉnh sửa
        // 2. Binding đến các control trong form
        // 3. Chứa tất cả thông tin của lớp học
        public Class Class
        {
            get => _class;
            set => SetProperty(ref _class, value);
        }

        // 1. Danh sách giáo viên có thể chọn
        // 2. Binding đến ComboBox giáo viên
        // 3. Được tải từ cơ sở dữ liệu khi khởi tạo form
        public ObservableCollection<TeacherViewModel> Teachers
        {
            get => _teachers;
            set => SetProperty(ref _teachers, value);
        }

        // 1. Giáo viên chủ nhiệm được chọn
        // 2. Binding đến SelectedItem của ComboBox giáo viên
        // 3. Cập nhật TeacherID của lớp học khi thay đổi
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

        // 1. Lệnh lưu lớp học
        // 2. Binding đến nút "Save" trong form
        // 3. Gọi phương thức SaveClass khi được kích hoạt
        public ICommand SaveCommand { get; }
        
        // 1. Lệnh hủy bỏ và đóng form
        // 2. Binding đến nút "Cancel" trong form
        // 3. Gọi phương thức CloseDialog khi được kích hoạt
        public ICommand CancelCommand { get; }

        // 1. Constructor cho chế độ thêm mới lớp học
        // 2. Khởi tạo ViewModel với các tham số cần thiết
        // 3. Tạo lớp học mới với các giá trị mặc định
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

        // 1. Constructor cho chế độ chỉnh sửa lớp học
        // 2. Khởi tạo ViewModel với lớp học cần chỉnh sửa
        // 3. Sao chép lớp học để tránh thay đổi trực tiếp
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

        // 1. Phương thức tải danh sách giáo viên từ cơ sở dữ liệu
        // 2. Thêm tùy chọn "No Teacher" và tất cả giáo viên
        // 3. Thiết lập giáo viên đã chọn dựa vào TeacherID của lớp học
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

        // 1. Phương thức kiểm tra lớp học có thể lưu không
        // 2. Xác thực các trường bắt buộc đã được nhập
        // 3. Dùng để kích hoạt/vô hiệu hóa nút Save
        private bool CanSaveClass()
        {
            return !string.IsNullOrWhiteSpace(Class.ClassName) && 
                   !string.IsNullOrWhiteSpace(Class.AcademicYear);
        }

        // 1. Phương thức lưu lớp học vào cơ sở dữ liệu
        // 2. Xác thực dữ liệu và thực hiện lưu hoặc cập nhật
        // 3. Hiển thị thông báo kết quả và đóng dialog
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

        // 1. Phương thức đóng hộp thoại
        // 2. Đặt DialogResult là false để biết người dùng đã hủy
        // 3. Được gọi khi nhấn nút Cancel hoặc đóng cửa sổ
        private void CloseDialog()
        {
            _dialogWindow.DialogResult = false;
            _dialogWindow.Close();
        }
    }

    // Lớp TeacherViewModel
    // + Tại sao cần sử dụng: Cung cấp cấu trúc dữ liệu đơn giản cho đối tượng giáo viên
    // + Lớp này được sử dụng trong danh sách Teachers và không liên kết trực tiếp với cơ sở dữ liệu
    // + Chức năng chính: Lưu trữ ID và tên đầy đủ của giáo viên để hiển thị trong ComboBox
    public class TeacherViewModel
    {
        // 1. ID của giáo viên
        // 2. Có thể null nếu là tùy chọn "No Teacher"
        // 3. Được sử dụng để cập nhật TeacherID của lớp học
        public int? TeacherID { get; set; }
        
        // 1. Tên đầy đủ của giáo viên
        // 2. Được hiển thị trong ComboBox
        // 3. Thường là kết hợp của FirstName và LastName
        public string FullName { get; set; } = string.Empty;
    }
}
