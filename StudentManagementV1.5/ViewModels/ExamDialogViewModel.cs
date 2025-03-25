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
    // Lớp ExamDialogViewModel
    // + Tại sao cần sử dụng: Quản lý giao diện và dữ liệu cho hộp thoại thêm/sửa kỳ thi
    // + Lớp này được gọi từ ExamManagementViewModel khi cần tạo hoặc chỉnh sửa kỳ thi
    // + Chức năng chính: Cung cấp các thuộc tính và lệnh để tạo mới hoặc cập nhật thông tin kỳ thi
    public class ExamDialogViewModel : ViewModelBase
    {
        // 1. Dịch vụ kết nối với cơ sở dữ liệu
        // 2. Dùng để truy vấn và cập nhật dữ liệu
        // 3. Được truyền vào từ constructor
        private readonly DatabaseService _databaseService;
        
        // 1. Cửa sổ hộp thoại hiện tại
        // 2. Dùng để điều khiển trạng thái của hộp thoại
        // 3. Được truyền vào từ constructor
        private readonly Window _dialogWindow;
        
        // 1. Lưu thông tin kỳ thi gốc nếu đang ở chế độ sửa
        // 2. Dùng để tham chiếu khi cập nhật
        // 3. Có thể null nếu đang ở chế độ thêm mới
        private readonly Exam? _originalExam;
        
        // 1. Xác định chế độ làm việc: chỉnh sửa hoặc thêm mới
        // 2. Dựa trên việc có truyền Exam vào constructor hay không
        // 3. True = chỉnh sửa, False = thêm mới
        private readonly bool _isEditMode;
        
        // 1. Trạng thái đang tải dữ liệu
        // 2. Dùng để hiển thị thông báo "Đang tải..."
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        private bool _isLoading;
        
        // 1. Tiêu đề của hộp thoại
        // 2. Hiển thị ở phần đầu của hộp thoại
        // 3. Được đặt dựa trên chế độ làm việc
        private string _dialogTitle;
        
        // 1. Tên của kỳ thi
        // 2. Hiển thị và chỉnh sửa trong form
        // 3. Bắt buộc phải điền
        private string _examName = string.Empty;
        
        // 1. Ngày diễn ra kỳ thi
        // 2. Hiển thị và chỉnh sửa trong form
        // 3. Mặc định là ngày hiện tại
        private DateTime _examDate = DateTime.Now;
        
        // 1. Thời gian làm bài thi (phút)
        // 2. Hiển thị và chỉnh sửa trong form
        // 3. Có thể null nếu không giới hạn thời gian
        private int? _duration;
        
        // 1. Tổng điểm của bài thi
        // 2. Hiển thị và chỉnh sửa trong form
        // 3. Mặc định là 100 điểm
        private int _totalMarks = 100;
        
        // 1. Mô tả về kỳ thi
        // 2. Hiển thị và chỉnh sửa trong form
        // 3. Có thể để trống
        private string _description = string.Empty;
        
        // 1. Danh sách môn học cho kỳ thi
        // 2. Hiển thị trong dropdown để chọn
        // 3. Được tải từ cơ sở dữ liệu
        private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();
        
        // 1. Danh sách lớp học cho kỳ thi
        // 2. Hiển thị trong dropdown để chọn
        // 3. Được tải từ cơ sở dữ liệu
        private ObservableCollection<Class> _classes = new ObservableCollection<Class>();
        
        // 1. Môn học được chọn cho kỳ thi
        // 2. Được chọn từ dropdown môn học
        // 3. Bắt buộc phải chọn
        private Subject? _selectedSubject;
        
        // 1. Lớp học được chọn cho kỳ thi
        // 2. Được chọn từ dropdown lớp học
        // 3. Bắt buộc phải chọn
        private Class? _selectedClass;

        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến UI để hiển thị thông báo "Đang tải..."
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // 1. Tiêu đề của hộp thoại
        // 2. Binding đến UI để hiển thị
        // 3. Được đặt dựa trên chế độ làm việc
        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetProperty(ref _dialogTitle, value);
        }

        // 1. Tên của kỳ thi
        // 2. Binding đến UI để hiển thị và chỉnh sửa
        // 3. Bắt buộc phải điền
        public string ExamName
        {
            get => _examName;
            set => SetProperty(ref _examName, value);
        }

        // 1. Ngày diễn ra kỳ thi
        // 2. Binding đến UI để hiển thị và chỉnh sửa
        // 3. Mặc định là ngày hiện tại
        public DateTime ExamDate
        {
            get => _examDate;
            set => SetProperty(ref _examDate, value);
        }

        // 1. Thời gian làm bài thi (phút)
        // 2. Binding đến UI để hiển thị và chỉnh sửa
        // 3. Có thể null nếu không giới hạn thời gian
        public int? Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        // 1. Tổng điểm của bài thi
        // 2. Binding đến UI để hiển thị và chỉnh sửa
        // 3. Mặc định là 100 điểm
        public int TotalMarks
        {
            get => _totalMarks;
            set => SetProperty(ref _totalMarks, value);
        }

        // 1. Mô tả về kỳ thi
        // 2. Binding đến UI để hiển thị và chỉnh sửa
        // 3. Có thể để trống
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        // 1. Danh sách môn học cho kỳ thi
        // 2. Binding đến ComboBox để hiển thị danh sách môn học
        // 3. Được tải từ cơ sở dữ liệu
        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set => SetProperty(ref _subjects, value);
        }

        // 1. Danh sách lớp học cho kỳ thi
        // 2. Binding đến ComboBox để hiển thị danh sách lớp học
        // 3. Được tải từ cơ sở dữ liệu
        public ObservableCollection<Class> Classes
        {
            get => _classes;
            set => SetProperty(ref _classes, value);
        }

        // 1. Môn học được chọn cho kỳ thi
        // 2. Binding đến ComboBox để hiển thị môn học đã chọn
        // 3. Bắt buộc phải chọn
        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set => SetProperty(ref _selectedSubject, value);
        }

        // 1. Lớp học được chọn cho kỳ thi
        // 2. Binding đến ComboBox để hiển thị lớp học đã chọn
        // 3. Bắt buộc phải chọn
        public Class? SelectedClass
        {
            get => _selectedClass;
            set => SetProperty(ref _selectedClass, value);
        }

        // 1. Lệnh lưu kỳ thi
        // 2. Binding đến nút Lưu trong UI
        // 3. Khi được gọi, sẽ thực hiện phương thức SaveExam
        public ICommand SaveCommand { get; }
        
        // 1. Lệnh hủy bỏ thao tác
        // 2. Binding đến nút Hủy trong UI
        // 3. Khi được gọi, sẽ đóng hộp thoại mà không lưu
        public ICommand CancelCommand { get; }

        // 1. Constructor của lớp
        // 2. Khởi tạo các tham số và trạng thái ban đầu
        // 3. Tải dữ liệu ban đầu từ cơ sở dữ liệu
        public ExamDialogViewModel(DatabaseService databaseService, Window dialogWindow, Exam? exam = null)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _originalExam = exam;
            _isEditMode = exam != null;

            // Set dialog title based on mode
            _dialogTitle = _isEditMode ? "Edit Exam" : "Add New Exam";

            // If editing, populate fields with existing exam data
            if (_isEditMode && exam != null)
            {
                _examName = exam.ExamName;
                _examDate = exam.ExamDate;
                _duration = exam.Duration;
                _totalMarks = exam.TotalMarks;
                _description = exam.Description;
            }

            // Commands
            SaveCommand = new RelayCommand(_ => SaveExam());
            CancelCommand = new RelayCommand(_ => _dialogWindow.DialogResult = false);

            // Load data
            InitializeDataAsync();
        }

        // 1. Phương thức khởi tạo dữ liệu
        // 2. Tải danh sách lớp học và môn học từ cơ sở dữ liệu
        // 3. Thiết lập giá trị đã chọn nếu đang ở chế độ sửa
        private async void InitializeDataAsync()
        {
            try
            {
                IsLoading = true;
                await LoadClassesAsync();
                await LoadSubjectsAsync();

                // If editing, set selected class and subject based on the exam
                if (_isEditMode && _originalExam != null)
                {
                    SelectedClass = Classes.FirstOrDefault(c => c.ClassID == _originalExam.ClassID);
                    SelectedSubject = Subjects.FirstOrDefault(s => s.SubjectID == _originalExam.SubjectID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // 1. Phương thức tải danh sách lớp học
        // 2. Truy vấn cơ sở dữ liệu để lấy thông tin lớp học
        // 3. Điền dữ liệu vào danh sách Classes
        private async Task LoadClassesAsync()
        {
            string query = "SELECT ClassID, ClassName FROM Classes WHERE IsActive = 1 ORDER BY ClassName";
            var result = await _databaseService.ExecuteQueryAsync(query);

            Classes.Clear();
            foreach (DataRow row in result.Rows)
            {
                Classes.Add(new Class
                {
                    ClassID = Convert.ToInt32(row["ClassID"]),
                    ClassName = row["ClassName"].ToString() ?? string.Empty
                });
            }

            if (Classes.Count > 0)
            {
                SelectedClass = Classes[0];
            }
        }

        // 1. Phương thức tải danh sách môn học
        // 2. Truy vấn cơ sở dữ liệu để lấy thông tin môn học
        // 3. Điền dữ liệu vào danh sách Subjects
        private async Task LoadSubjectsAsync()
        {
            string query = "SELECT SubjectID, SubjectName FROM Subjects WHERE IsActive = 1 ORDER BY SubjectName";
            var result = await _databaseService.ExecuteQueryAsync(query);

            Subjects.Clear();
            foreach (DataRow row in result.Rows)
            {
                Subjects.Add(new Subject
                {
                    SubjectID = Convert.ToInt32(row["SubjectID"]),
                    SubjectName = row["SubjectName"].ToString() ?? string.Empty
                });
            }

            if (Subjects.Count > 0)
            {
                SelectedSubject = Subjects[0];
            }
        }

        // 1. Phương thức lưu kỳ thi
        // 2. Kiểm tra tính hợp lệ của dữ liệu
        // 3. Tạo mới hoặc cập nhật kỳ thi trong cơ sở dữ liệu
        private async void SaveExam()
        {
            if (!ValidateExam())
            {
                return;
            }

            try
            {
                IsLoading = true;

                if (_isEditMode)
                {
                    await UpdateExamAsync();
                }
                else
                {
                    await CreateExamAsync();
                }

                _dialogWindow.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving exam: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                IsLoading = false;
            }
        }

        // 1. Phương thức kiểm tra tính hợp lệ của dữ liệu kỳ thi
        // 2. Kiểm tra các trường bắt buộc và giá trị hợp lệ
        // 3. Hiển thị thông báo lỗi nếu có
        private bool ValidateExam()
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(ExamName))
            {
                MessageBox.Show("Please enter an exam name.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedSubject == null)
            {
                MessageBox.Show("Please select a subject.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedClass == null)
            {
                MessageBox.Show("Please select a class.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (TotalMarks <= 0)
            {
                MessageBox.Show("Total marks must be greater than zero.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Duration.HasValue && Duration.Value <= 0)
            {
                MessageBox.Show("Duration must be greater than zero.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // 1. Phương thức tạo kỳ thi mới
        // 2. Thực hiện truy vấn INSERT vào cơ sở dữ liệu
        // 3. Hiển thị thông báo kết quả
        private async Task CreateExamAsync()
        {
            // Fixed: Removed CreatedAt column which doesn't exist in the database
            string query = @"
                INSERT INTO Exams (
                    SubjectID, 
                    ClassID, 
                    ExamName, 
                    ExamDate, 
                    Duration, 
                    TotalMarks, 
                    Description
                ) 
                VALUES (
                    @SubjectID, 
                    @ClassID, 
                    @ExamName, 
                    @ExamDate, 
                    @Duration, 
                    @TotalMarks, 
                    @Description
                )";

            var parameters = new Dictionary<string, object>
            {
                { "@SubjectID", SelectedSubject!.SubjectID },
                { "@ClassID", SelectedClass!.ClassID },
                { "@ExamName", ExamName },
                { "@ExamDate", ExamDate },
                { "@TotalMarks", TotalMarks },
                { "@Description", Description ?? string.Empty }
            };

            if (Duration.HasValue)
            {
                parameters.Add("@Duration", Duration.Value);
            }
            else
            {
                parameters.Add("@Duration", DBNull.Value);
            }

            await _databaseService.ExecuteNonQueryAsync(query, parameters);
            MessageBox.Show("Exam created successfully!",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 1. Phương thức cập nhật kỳ thi
        // 2. Thực hiện truy vấn UPDATE vào cơ sở dữ liệu
        // 3. Hiển thị thông báo kết quả
        private async Task UpdateExamAsync()
        {
            if (_originalExam == null) return;

            // Fixed: Removed UpdatedAt column which doesn't exist in the database
            string query = @"
                UPDATE Exams 
                SET 
                    SubjectID = @SubjectID,
                    ClassID = @ClassID,
                    ExamName = @ExamName,
                    ExamDate = @ExamDate,
                    Duration = @Duration,
                    TotalMarks = @TotalMarks,
                    Description = @Description
                WHERE 
                    ExamID = @ExamID";

            var parameters = new Dictionary<string, object>
            {
                { "@ExamID", _originalExam.ExamID },
                { "@SubjectID", SelectedSubject!.SubjectID },
                { "@ClassID", SelectedClass!.ClassID },
                { "@ExamName", ExamName },
                { "@ExamDate", ExamDate },
                { "@TotalMarks", TotalMarks },
                { "@Description", Description ?? string.Empty }
            };

            if (Duration.HasValue)
            {
                parameters.Add("@Duration", Duration.Value);
            }
            else
            {
                parameters.Add("@Duration", DBNull.Value);
            }

            await _databaseService.ExecuteNonQueryAsync(query, parameters);
            MessageBox.Show("Exam updated successfully!",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
