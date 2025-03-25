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
    // Lớp ClassManagementViewModel
    // + Tại sao cần sử dụng: Cung cấp dữ liệu và xử lý logic cho màn hình quản lý lớp học
    // + Lớp này được gọi từ NavigationService khi chuyển đến màn hình quản lý lớp học
    // + Chức năng chính: Hiển thị, tìm kiếm, thêm, sửa và xóa lớp học
    public class ClassManagementViewModel : ViewModelBase
    {
        // 1. Hằng số cho tên cột TeacherID trong truy vấn
        // 2. Giúp đảm bảo tính nhất quán khi tham chiếu đến cột này
        // 3. Tránh lỗi do gõ sai tên cột
        private const string TeacherIdColumn = "TeacherID";
        
        // 1. Hằng số cho tên cột ClassID trong truy vấn
        // 2. Giúp đảm bảo tính nhất quán khi tham chiếu đến cột này
        // 3. Tránh lỗi do gõ sai tên cột
        private const string ClassIdColumn = "ClassID";
        
        // 1. Hằng số cho tên cột TeacherName trong truy vấn
        // 2. Giúp đảm bảo tính nhất quán khi tham chiếu đến cột này
        // 3. Tránh lỗi do gõ sai tên cột
        private const string TeacherNameColumn = "TeacherName";
        
        // 1. Dịch vụ kết nối với cơ sở dữ liệu
        // 2. Dùng để truy vấn và cập nhật dữ liệu lớp học
        // 3. Được truyền vào từ constructor
        private readonly DatabaseService _databaseService;
        
        // 1. Dịch vụ điều hướng
        // 2. Dùng để chuyển đổi giữa các màn hình
        // 3. Được truyền vào từ constructor
        private readonly NavigationService _navigationService;

        // 1. Danh sách các lớp học
        // 2. Hiển thị trong DataGrid
        // 3. Được tải từ cơ sở dữ liệu
        private ObservableCollection<Class> _classes = [];
        
        // 1. Từ khóa tìm kiếm
        // 2. Binding đến TextBox tìm kiếm
        // 3. Khi thay đổi sẽ lọc danh sách lớp học
        private string _searchText = string.Empty;
        
        // 1. Tùy chọn hiển thị lớp không hoạt động
        // 2. Binding đến CheckBox trong UI
        // 3. Khi thay đổi sẽ lọc danh sách lớp học
        private bool _showInactiveClasses = false;
        
        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến UI để hiển thị thông báo "Đang tải..."
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        private bool _isLoading;
        
        // 1. Lớp học được chọn
        // 2. Binding đến DataGrid.SelectedItem
        // 3. Dùng cho các thao tác sửa và xóa
        private Class? _selectedClass;

        // 1. Danh sách các lớp học
        // 2. Binding đến DataGrid để hiển thị
        // 3. Được tải từ cơ sở dữ liệu
        public ObservableCollection<Class> Classes
        {
            get => _classes;
            set => SetProperty(ref _classes, value);
        }

        // 1. Từ khóa tìm kiếm
        // 2. Binding đến TextBox tìm kiếm
        // 3. Khi thay đổi sẽ lọc danh sách lớp học
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

        // 1. Tùy chọn hiển thị lớp không hoạt động
        // 2. Binding đến CheckBox trong UI
        // 3. Khi thay đổi sẽ lọc danh sách lớp học
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

        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến UI để hiển thị thông báo "Đang tải..."
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // 1. Lớp học được chọn
        // 2. Binding đến DataGrid.SelectedItem
        // 3. Dùng cho các thao tác sửa và xóa
        public Class? SelectedClass
        {
            get => _selectedClass;
            set => SetProperty(ref _selectedClass, value);
        }

        // 1. Lệnh thêm lớp học mới
        // 2. Binding đến nút "Thêm lớp học" trong UI
        // 3. Khi được gọi, mở hộp thoại thêm lớp học
        public ICommand AddClassCommand { get; }
        
        // 1. Lệnh sửa lớp học
        // 2. Binding đến nút "Sửa" trong UI
        // 3. Khi được gọi, mở hộp thoại sửa lớp học với lớp đã chọn
        public ICommand EditClassCommand { get; }
        
        // 1. Lệnh xóa lớp học
        // 2. Binding đến nút "Xóa" trong UI
        // 3. Khi được gọi, hiển thị xác nhận và xóa lớp học đã chọn
        public ICommand DeleteClassCommand { get; }
        
        // 1. Lệnh quay lại
        // 2. Binding đến nút "Quay lại" trong UI
        // 3. Khi được gọi, chuyển về màn hình dashboard
        public ICommand BackCommand { get; }

        // 1. Constructor của lớp
        // 2. Khởi tạo các tham số và thiết lập lệnh
        // 3. Tải dữ liệu ban đầu từ cơ sở dữ liệu
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
        
        // 1. Phương thức làm mới danh sách lớp học
        // 2. Gọi khi có thay đổi tìm kiếm hoặc bộ lọc
        // 3. Tải lại dữ liệu từ cơ sở dữ liệu
        private async Task RefreshClassesAsync()
        {
            await LoadClassesAsync();
        }
        
        // 1. Phương thức tải danh sách lớp học từ cơ sở dữ liệu
        // 2. Truy vấn dữ liệu dựa trên điều kiện tìm kiếm và bộ lọc
        // 3. Điền dữ liệu vào danh sách Classes
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
        
        // 1. Phương thức xây dựng câu truy vấn SQL
        // 2. Tạo câu truy vấn dựa trên điều kiện tìm kiếm và bộ lọc
        // 3. Trả về chuỗi truy vấn SQL hoàn chỉnh
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
        
        // 1. Phương thức điền dữ liệu từ DataTable vào danh sách Classes
        // 2. Chuyển đổi từng dòng dữ liệu thành đối tượng Class
        // 3. Thêm đối tượng Class vào danh sách Classes
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
        
        // 1. Phương thức mở hộp thoại thêm lớp học
        // 2. Tạo cửa sổ AddEditClassView với ViewModel tương ứng
        // 3. Nếu lưu thành công, làm mới danh sách lớp học
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
        
        // 1. Phương thức mở hộp thoại sửa lớp học
        // 2. Tạo cửa sổ AddEditClassView với ViewModel và dữ liệu lớp học cần sửa
        // 3. Nếu lưu thành công, làm mới danh sách lớp học
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
        
        // 1. Phương thức xóa lớp học
        // 2. Hiển thị xác nhận và gọi phương thức hủy kích hoạt lớp học
        // 3. Nếu xác nhận, lớp học sẽ bị đánh dấu là không hoạt động
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
        
        // 1. Phương thức hủy kích hoạt lớp học
        // 2. Kiểm tra xem lớp có học sinh không và cập nhật trạng thái
        // 3. Nếu thành công, làm mới danh sách lớp học
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
