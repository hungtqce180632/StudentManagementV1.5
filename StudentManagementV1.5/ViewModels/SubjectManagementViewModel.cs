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
    // Lớp SubjectManagementViewModel
    // + Tại sao cần sử dụng: Quản lý và hiển thị danh sách môn học trong hệ thống
    // + Lớp này được gọi từ màn hình quản lý môn học (SubjectManagementView)
    // + Chức năng chính: Tìm kiếm, lọc, thêm, sửa và xóa môn học
    public class SubjectManagementViewModel : ViewModelBase
    {
        // 1. Dịch vụ cơ sở dữ liệu để truy vấn và cập nhật thông tin môn học
        // 2. Được truyền vào từ constructor
        // 3. Dùng để tải và thao tác với dữ liệu môn học
        private readonly DatabaseService _databaseService;

        // 1. Dịch vụ điều hướng
        // 2. Được truyền vào từ constructor
        // 3. Dùng để chuyển đổi giữa các màn hình
        private readonly NavigationService _navigationService;

        // 1. Danh sách môn học hiển thị trong UI
        // 2. Binding đến DataGrid hoặc ListView
        // 3. Được tải từ cơ sở dữ liệu và lọc theo các điều kiện
        private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();

        // 1. Từ khóa tìm kiếm
        // 2. Binding đến TextBox tìm kiếm
        // 3. Dùng để lọc môn học theo tên hoặc mô tả
        private string _searchText = string.Empty;

        // 1. Tùy chọn hiển thị môn học không hoạt động
        // 2. Binding đến CheckBox trong UI
        // 3. Khi thay đổi sẽ lọc danh sách môn học
        private bool _showInactiveSubjects = false;

        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến indicator trong UI
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        private bool _isLoading;

        // 1. Môn học được chọn trong danh sách
        // 2. Binding đến SelectedItem của DataGrid/ListView
        // 3. Dùng cho các thao tác sửa, xóa hoặc hiển thị chi tiết
        private Subject? _selectedSubject;

        // 1. Danh sách môn học hiển thị trong UI
        // 2. Binding đến DataGrid hoặc ListView
        // 3. Được tải từ cơ sở dữ liệu và lọc theo các điều kiện
        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set => SetProperty(ref _subjects, value);
        }

        // 1. Từ khóa tìm kiếm
        // 2. Binding đến TextBox tìm kiếm
        // 3. Khi thay đổi sẽ gọi RefreshSubjectsAsync để cập nhật danh sách
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

        // 1. Tùy chọn hiển thị môn học không hoạt động
        // 2. Binding đến CheckBox trong UI
        // 3. Khi thay đổi sẽ gọi RefreshSubjectsAsync để cập nhật danh sách
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

        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến indicator trong UI
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // 1. Môn học được chọn trong danh sách
        // 2. Binding đến SelectedItem của DataGrid/ListView
        // 3. Dùng cho các thao tác sửa, xóa hoặc hiển thị chi tiết
        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set => SetProperty(ref _selectedSubject, value);
        }

        // 1. Lệnh thêm môn học mới
        // 2. Binding đến nút "Thêm môn học" trong UI
        // 3. Khi được gọi, mở hộp thoại thêm môn học
        public ICommand AddSubjectCommand { get; }

        // 1. Lệnh sửa môn học
        // 2. Binding đến nút "Sửa" trong UI
        // 3. Khi được gọi, mở hộp thoại sửa môn học với thông tin đã chọn
        public ICommand EditSubjectCommand { get; }

        // 1. Lệnh xóa môn học
        // 2. Binding đến nút "Xóa" trong UI
        // 3. Khi được gọi, hiển thị xác nhận và xóa môn học đã chọn
        public ICommand DeleteSubjectCommand { get; }

        // 1. Lệnh quay lại
        // 2. Binding đến nút "Quay lại" trong UI
        // 3. Khi được gọi, chuyển về màn hình dashboard
        public ICommand BackCommand { get; }

        // 1. Constructor của lớp
        // 2. Khởi tạo các tham số và thiết lập lệnh
        // 3. Tải dữ liệu ban đầu từ cơ sở dữ liệu
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

        // 1. Phương thức làm mới danh sách môn học
        // 2. Gọi khi có thay đổi tìm kiếm hoặc bộ lọc
        // 3. Tải lại dữ liệu từ cơ sở dữ liệu
        private async Task RefreshSubjectsAsync()
        {
            await LoadSubjectsAsync();
        }

        // 1. Phương thức tải danh sách môn học
        // 2. Truy vấn cơ sở dữ liệu với các điều kiện lọc
        // 3. Cập nhật danh sách Subjects với kết quả tìm kiếm
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

        // 1. Phương thức xây dựng câu truy vấn SQL
        // 2. Tạo câu truy vấn dựa trên điều kiện tìm kiếm và bộ lọc
        // 3. Trả về chuỗi truy vấn SQL hoàn chỉnh
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

        // 1. Phương thức tạo tham số cho truy vấn
        // 2. Tạo dictionary chứa các tham số truy vấn
        // 3. Trả về dictionary với tham số tìm kiếm (nếu có)
        private Dictionary<string, object> BuildQueryParameters()
        {
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                parameters["@Search"] = $"%{_searchText}%";
            }

            return parameters;
        }

        // 1. Phương thức điền dữ liệu từ DataTable vào danh sách Subjects
        // 2. Chuyển đổi từng dòng dữ liệu thành đối tượng Subject
        // 3. Thêm đối tượng Subject vào danh sách Subjects
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

        // 1. Phương thức thêm môn học mới
        // 2. Mở hộp thoại AddSubjectView để thêm môn học
        private void AddSubject()
        {
            var dialog = new AddEditSubjectView();
            dialog.DataContext = new AddEditSubjectViewModel(_databaseService, dialog);
            dialog.Owner = Application.Current.MainWindow;

            if (dialog.ShowDialog() == true)
            {
                // Refresh the subject list after adding
                _ = RefreshSubjectsAsync();
            }
        }

        // 1. Phương thức sửa môn học
        // 2. Hiển thị thông báo chức năng sẽ được triển khai trong tương lai
        // 3. Sẽ mở hộp thoại sửa với thông tin môn học đã chọn trong triển khai tương lai
        private void EditSubject(Subject? subject)
        {
            if (subject == null) return;

            var dialog = new AddEditSubjectView();
            var viewModel = new AddEditSubjectViewModel(_databaseService, dialog, subject);
            dialog.DataContext = viewModel;
            dialog.Owner = Application.Current.MainWindow;

            if (dialog.ShowDialog() == true)
            {
                // Refresh the subject list after editing
                _ = RefreshSubjectsAsync();
            }
        }

        // 1. Phương thức xóa môn học
        // 2. Hiển thị hộp thoại xác nhận trước khi xóa
        // 3. Nếu xác nhận, gọi phương thức hủy kích hoạt môn học
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

        // 1. Phương thức hủy kích hoạt môn học
        // 2. Kiểm tra xem môn học có đang được sử dụng không và cập nhật trạng thái
        // 3. Nếu thành công, làm mới danh sách môn học
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
