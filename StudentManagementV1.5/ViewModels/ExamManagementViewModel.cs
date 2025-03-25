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
using System.Windows.Threading;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp ExamManagementViewModel
    // + Tại sao cần sử dụng: Quản lý danh sách, tìm kiếm, lọc, tạo, sửa và xóa kỳ thi
    // + Lớp này được gọi từ màn hình quản lý kỳ thi
    // + Chức năng chính: Hiển thị và quản lý các kỳ thi trong hệ thống
    public class ExamManagementViewModel : ViewModelBase
    {
        // 1. Dịch vụ cơ sở dữ liệu để truy vấn và cập nhật kỳ thi
        // 2. Được truyền vào từ constructor
        // 3. Dùng để tải và thao tác với dữ liệu kỳ thi
        private readonly DatabaseService _databaseService;
        
        // 1. Dịch vụ điều hướng
        // 2. Được truyền vào từ constructor
        // 3. Sử dụng để điều hướng giữa các màn hình
        private readonly NavigationService _navigationService;

        // 1. Danh sách kỳ thi hiển thị trong UI
        // 2. Binding đến ListView hoặc DataGrid
        // 3. Được tải từ cơ sở dữ liệu và lọc theo các điều kiện
        private ObservableCollection<Exam> _exams = new ObservableCollection<Exam>();
        
        // 1. Danh sách lớp học để lọc kỳ thi
        // 2. Binding đến ComboBox lọc theo lớp
        // 3. Được tải từ cơ sở dữ liệu
        private ObservableCollection<Class> _classes = new ObservableCollection<Class>();
        
        // 1. Danh sách môn học để lọc kỳ thi
        // 2. Binding đến ComboBox lọc theo môn học
        // 3. Được tải từ cơ sở dữ liệu
        private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();
        
        // 1. Từ khóa tìm kiếm
        // 2. Binding đến TextBox tìm kiếm
        // 3. Dùng để lọc kỳ thi theo tên hoặc mô tả
        private string _searchText = string.Empty;
        
        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến indicator trong UI
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        private bool _isLoading;
        
        // 1. Kỳ thi được chọn trong danh sách
        // 2. Binding đến SelectedItem của ListView/DataGrid
        // 3. Dùng cho các thao tác xem chi tiết, sửa, xóa
        private Exam? _selectedExam;
        
        // 1. Lớp học được chọn để lọc
        // 2. Binding đến SelectedItem của ComboBox lớp học
        // 3. Khi thay đổi sẽ lọc danh sách kỳ thi
        private Class? _selectedClass;
        
        // 1. Môn học được chọn để lọc
        // 2. Binding đến SelectedItem của ComboBox môn học
        // 3. Khi thay đổi sẽ lọc danh sách kỳ thi
        private Subject? _selectedSubject;

        // 1. Danh sách kỳ thi hiển thị trong UI
        // 2. Binding đến ListView hoặc DataGrid
        // 3. Được tải từ cơ sở dữ liệu và lọc theo các điều kiện
        public ObservableCollection<Exam> Exams
        {
            get => _exams;
            set => SetProperty(ref _exams, value);
        }

        // 1. Danh sách lớp học để lọc kỳ thi
        // 2. Binding đến ComboBox lọc theo lớp
        // 3. Được tải từ cơ sở dữ liệu
        public ObservableCollection<Class> Classes
        {
            get => _classes;
            set => SetProperty(ref _classes, value);
        }

        // 1. Danh sách môn học để lọc kỳ thi
        // 2. Binding đến ComboBox lọc theo môn học
        // 3. Được tải từ cơ sở dữ liệu
        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set => SetProperty(ref _subjects, value);
        }

        // 1. Từ khóa tìm kiếm
        // 2. Binding đến TextBox tìm kiếm
        // 3. Dùng để lọc kỳ thi theo tên hoặc mô tả
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = RefreshExamsAsync();
                }
            }
        }

        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến indicator trong UI
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // 1. Kỳ thi được chọn trong danh sách
        // 2. Binding đến SelectedItem của ListView/DataGrid
        // 3. Dùng cho các thao tác xem chi tiết, sửa, xóa
        public Exam? SelectedExam
        {
            get => _selectedExam;
            set => SetProperty(ref _selectedExam, value);
        }

        // 1. Lớp học được chọn để lọc
        // 2. Binding đến SelectedItem của ComboBox lớp học
        // 3. Khi thay đổi sẽ lọc danh sách kỳ thi
        public Class? SelectedClass
        {
            get => _selectedClass;
            set
            {
                if (SetProperty(ref _selectedClass, value))
                {
                    _ = RefreshExamsAsync();
                }
            }
        }

        // 1. Môn học được chọn để lọc
        // 2. Binding đến SelectedItem của ComboBox môn học
        // 3. Khi thay đổi sẽ lọc danh sách kỳ thi
        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                if (SetProperty(ref _selectedSubject, value))
                {
                    _ = RefreshExamsAsync();
                }
            }
        }

        // 1. Lệnh thêm kỳ thi mới
        // 2. Binding đến nút "Thêm kỳ thi" trong UI
        // 3. Mở hộp thoại thêm kỳ thi mới
        public ICommand AddExamCommand { get; }
        
        // 1. Lệnh chỉnh sửa kỳ thi
        // 2. Binding đến nút "Sửa" trong UI
        // 3. Mở hộp thoại chỉnh sửa kỳ thi đã chọn
        public ICommand EditExamCommand { get; }
        
        // 1. Lệnh xóa kỳ thi
        // 2. Binding đến nút "Xóa" trong UI
        // 3. Xóa kỳ thi đã chọn sau khi xác nhận
        public ICommand DeleteExamCommand { get; }
        
        // 1. Lệnh xem điểm số của kỳ thi
        // 2. Binding đến nút "Xem điểm" trong UI
        // 3. Điều hướng đến màn hình quản lý điểm số của kỳ thi
        public ICommand ViewScoresCommand { get; }
        
        // 1. Lệnh quay lại màn hình trước
        // 2. Binding đến nút "Quay lại" trong UI
        // 3. Điều hướng về màn hình dashboard
        public ICommand BackCommand { get; }

        // 1. Constructor của lớp
        // 2. Khởi tạo các tham số và thiết lập lệnh
        // 3. Tải dữ liệu ban đầu từ cơ sở dữ liệu
        public ExamManagementViewModel(DatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;

            AddExamCommand = new RelayCommand(_ => AddExam());
            EditExamCommand = new RelayCommand(param => EditExam(param as Exam), param => param != null);
            DeleteExamCommand = new RelayCommand(async param => await DeleteExamAsync(param as Exam), param => param != null);
            ViewScoresCommand = new RelayCommand(param => ViewScores(param as Exam), param => param != null);
            BackCommand = new RelayCommand(_ => _navigationService.NavigateTo(AppViews.AdminDashboard));

            // Load initial data - don't use Task.Run which creates a background thread
            InitializeDataAsync();
        }

        // 1. Phương thức khởi tạo dữ liệu ban đầu
        // 2. Tải lớp học, môn học và kỳ thi theo thứ tự
        // 3. Xử lý và hiển thị lỗi nếu có
        private async void InitializeDataAsync()
        {
            try
            {
                await LoadClassesAsync();
                await LoadSubjectsAsync();
                await LoadExamsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing data: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 1. Phương thức làm mới danh sách kỳ thi
        // 2. Gọi khi có thay đổi về bộ lọc hoặc tìm kiếm
        // 3. Tải lại dữ liệu kỳ thi từ cơ sở dữ liệu
        private async Task RefreshExamsAsync()
        {
            await LoadExamsAsync();
        }

        // 1. Phương thức tải danh sách lớp học
        // 2. Thêm tùy chọn "Tất cả lớp học" và tải danh sách từ cơ sở dữ liệu
        // 3. Cập nhật danh sách Classes và thiết lập lớp được chọn mặc định
        private async Task LoadClassesAsync()
        {
            try
            {
                IsLoading = true;

                // Load classes from database
                string query = "SELECT ClassID, ClassName FROM Classes WHERE IsActive = 1 ORDER BY ClassName";
                var result = await _databaseService.ExecuteQueryAsync(query);

                // Update UI on the dispatcher thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Classes.Clear();
                    // Add "All Classes" option
                    Classes.Add(new Class { ClassID = 0, ClassName = "All Classes" });

                    foreach (DataRow row in result.Rows)
                    {
                        Classes.Add(new Class
                        {
                            ClassID = Convert.ToInt32(row["ClassID"]),
                            ClassName = row["ClassName"].ToString() ?? string.Empty
                        });
                    }

                    // Set default selection
                    SelectedClass = Classes[0]; // "All Classes"
                });
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

        // 1. Phương thức tải danh sách môn học
        // 2. Thêm tùy chọn "Tất cả môn học" và tải danh sách từ cơ sở dữ liệu
        // 3. Cập nhật danh sách Subjects và thiết lập môn học được chọn mặc định
        private async Task LoadSubjectsAsync()
        {
            try
            {
                IsLoading = true;

                // Load subjects from database
                string query = "SELECT SubjectID, SubjectName FROM Subjects WHERE IsActive = 1 ORDER BY SubjectName";
                var result = await _databaseService.ExecuteQueryAsync(query);

                // Update UI on the dispatcher thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Subjects.Clear();
                    // Add "All Subjects" option
                    Subjects.Add(new Subject { SubjectID = 0, SubjectName = "All Subjects" });

                    foreach (DataRow row in result.Rows)
                    {
                        Subjects.Add(new Subject
                        {
                            SubjectID = Convert.ToInt32(row["SubjectID"]),
                            SubjectName = row["SubjectName"].ToString() ?? string.Empty
                        });
                    }

                    // Set default selection
                    SelectedSubject = Subjects[0]; // "All Subjects"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // 1. Phương thức tải danh sách kỳ thi
        // 2. Tạo câu truy vấn và tham số dựa trên bộ lọc
        // 3. Cập nhật danh sách Exams với kết quả từ cơ sở dữ liệu
        private async Task LoadExamsAsync()
        {
            try
            {
                IsLoading = true;

                string query = BuildExamQuery();
                var parameters = BuildQueryParameters();
                DataTable result = await _databaseService.ExecuteQueryAsync(query, parameters);

                // Update UI on the dispatcher thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Exams.Clear();
                    PopulateExamsFromDataTable(result);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading exams: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // 1. Phương thức xây dựng câu truy vấn SQL
        // 2. Thêm điều kiện lọc dựa trên lớp học, môn học và từ khóa tìm kiếm
        // 3. Trả về chuỗi truy vấn SQL hoàn chỉnh
        private string BuildExamQuery()
        {
            string query = @"
                SELECT 
                    e.ExamID, 
                    e.ExamName, 
                    e.SubjectID,
                    s.SubjectName,
                    e.ClassID,
                    c.ClassName,
                    e.ExamDate, 
                    e.Duration, 
                    e.TotalMarks, 
                    e.Description
                FROM Exams e
                JOIN Subjects s ON e.SubjectID = s.SubjectID
                JOIN Classes c ON e.ClassID = c.ClassID
                WHERE 1=1";

            if (_selectedClass?.ClassID > 0)
            {
                query += " AND e.ClassID = @ClassID";
            }

            if (_selectedSubject?.SubjectID > 0)
            {
                query += " AND e.SubjectID = @SubjectID";
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                query += " AND (e.ExamName LIKE @Search OR e.Description LIKE @Search)";
            }

            query += " ORDER BY e.ExamDate DESC";

            return query;
        }

        // 1. Phương thức xây dựng tham số cho truy vấn
        // 2. Thêm tham số cho điều kiện lọc đã chọn
        // 3. Trả về dictionary chứa các tham số và giá trị
        private Dictionary<string, object> BuildQueryParameters()
        {
            var parameters = new Dictionary<string, object>();

            if (_selectedClass?.ClassID > 0)
            {
                parameters["@ClassID"] = _selectedClass.ClassID;
            }

            if (_selectedSubject?.SubjectID > 0)
            {
                parameters["@SubjectID"] = _selectedSubject.SubjectID;
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                parameters["@Search"] = $"%{_searchText}%";
            }

            return parameters;
        }

        // 1. Phương thức chuyển đổi dữ liệu từ DataTable sang các đối tượng Exam
        // 2. Đọc từng dòng và tạo đối tượng Exam tương ứng
        // 3. Thêm các đối tượng Exam vào danh sách Exams
        private void PopulateExamsFromDataTable(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Exams.Add(new Exam
                {
                    ExamID = Convert.ToInt32(row["ExamID"]),
                    ExamName = row["ExamName"].ToString() ?? string.Empty,
                    SubjectID = Convert.ToInt32(row["SubjectID"]),
                    SubjectName = row["SubjectName"].ToString() ?? string.Empty,
                    ClassID = Convert.ToInt32(row["ClassID"]),
                    ClassName = row["ClassName"].ToString() ?? string.Empty,
                    ExamDate = Convert.ToDateTime(row["ExamDate"]),
                    Duration = row["Duration"] != DBNull.Value ? Convert.ToInt32(row["Duration"]) : null,
                    TotalMarks = Convert.ToInt32(row["TotalMarks"]),
                    Description = row["Description"]?.ToString() ?? string.Empty
                });
            }
        }

        // 1. Phương thức thêm kỳ thi mới
        // 2. Mở hộp thoại ExamDialogView để nhập thông tin kỳ thi
        // 3. Làm mới danh sách kỳ thi nếu thêm thành công
        private void AddExam()
        {
            // Create and show the dialog
            var dialogWindow = new ExamDialogView();
            var viewModel = new ExamDialogViewModel(_databaseService, dialogWindow);
            dialogWindow.DataContext = viewModel;
            dialogWindow.Owner = Application.Current.MainWindow;

            // If dialog was confirmed, refresh the exams list
            if (dialogWindow.ShowDialog() == true)
            {
                _ = RefreshExamsAsync();
            }
        }

        // 1. Phương thức chỉnh sửa kỳ thi đã chọn
        // 2. Mở hộp thoại ExamDialogView với thông tin kỳ thi cần sửa
        // 3. Làm mới danh sách kỳ thi nếu sửa thành công
        private void EditExam(Exam? exam)
        {
            if (exam == null) return;

            // Create and show the dialog with the exam data
            var dialogWindow = new ExamDialogView();
            var viewModel = new ExamDialogViewModel(_databaseService, dialogWindow, exam);
            dialogWindow.DataContext = viewModel;
            dialogWindow.Owner = Application.Current.MainWindow;

            // If dialog was confirmed, refresh the exams list
            if (dialogWindow.ShowDialog() == true)
            {
                _ = RefreshExamsAsync();
            }
        }

        // 1. Phương thức xóa kỳ thi
        // 2. Hiển thị xác nhận trước khi xóa
        // 3. Gọi phương thức DeleteExamFromDatabaseAsync nếu người dùng xác nhận
        private async Task DeleteExamAsync(Exam? exam)
        {
            if (exam == null) return;

            // Confirm deletion
            var result = MessageBox.Show($"Are you sure you want to delete exam '{exam.ExamName}'?\n\nThis will also delete all associated scores.",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteExamFromDatabaseAsync(exam);
            }
        }

        // 1. Phương thức xóa kỳ thi từ cơ sở dữ liệu
        // 2. Kiểm tra và xóa điểm số liên quan trước khi xóa kỳ thi
        // 3. Làm mới danh sách sau khi xóa thành công
        private async Task DeleteExamFromDatabaseAsync(Exam exam)
        {
            try
            {
                IsLoading = true;

                // Check if exam has scores
                string checkQuery = "SELECT COUNT(*) FROM Scores WHERE ExamID = @ExamID";
                var checkParams = new Dictionary<string, object> { { "@ExamID", exam.ExamID } };
                var scoreCount = await _databaseService.ExecuteScalarAsync(checkQuery, checkParams);

                if (scoreCount != null && Convert.ToInt32(scoreCount) > 0)
                {
                    var confirmDelete = MessageBox.Show(
                        $"This exam has {scoreCount} score records associated with it. Deleting the exam will also delete all scores. Continue?",
                        "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (confirmDelete != MessageBoxResult.Yes)
                    {
                        return;
                    }

                    // Delete associated scores first
                    string deleteScoresQuery = "DELETE FROM Scores WHERE ExamID = @ExamID";
                    await _databaseService.ExecuteNonQueryAsync(deleteScoresQuery, checkParams);
                }

                // Delete the exam
                string query = "DELETE FROM Exams WHERE ExamID = @ExamID";
                var parameters = new Dictionary<string, object>
                {
                    { "@ExamID", exam.ExamID }
                };

                await _databaseService.ExecuteNonQueryAsync(query, parameters);

                // Refresh the list
                await LoadExamsAsync();

                MessageBox.Show($"Exam '{exam.ExamName}' has been deleted.",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting exam: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // 1. Phương thức xem điểm số của kỳ thi
        // 2. Hiển thị thông báo chức năng sẽ được triển khai trong tương lai
        // 3. Sẽ điều hướng đến màn hình quản lý điểm số trong triển khai tương lai
        private void ViewScores(Exam? exam)
        {
            if (exam == null) return;

            // Navigate to the score management view with the selected exam
            MessageBox.Show($"View scores for '{exam.ExamName}' functionality will be implemented in a future update.",
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);

            // In future implementation:
            // _navigationService.NavigateTo(AppViews.ScoreManagement, exam);
        }
    }
}
