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
    // Lớp NotificationManagementViewModel
    // + Tại sao cần sử dụng: Quản lý và hiển thị danh sách thông báo trong hệ thống
    // + Lớp này được gọi từ màn hình quản lý thông báo (NotificationManagementView)
    // + Chức năng chính: Tìm kiếm, lọc, thêm, sửa và xóa thông báo
    public class NotificationManagementViewModel : ViewModelBase
    {
        // 1. Dịch vụ cơ sở dữ liệu để truy vấn và cập nhật thông tin thông báo
        // 2. Được truyền vào từ constructor
        // 3. Dùng để tải và thao tác với dữ liệu thông báo
        private readonly DatabaseService _databaseService;
        
        // 1. Dịch vụ xác thực người dùng
        // 2. Được truyền vào từ constructor
        // 3. Dùng để lấy thông tin người dùng hiện tại và kiểm tra quyền
        private readonly AuthenticationService _authService;
        
        // 1. Dịch vụ điều hướng
        // 2. Được truyền vào từ constructor
        // 3. Dùng để chuyển đổi giữa các màn hình
        private readonly NavigationService _navigationService;

        // 1. Danh sách thông báo hiển thị trong UI
        // 2. Binding đến DataGrid hoặc ListView 
        // 3. Được tải từ cơ sở dữ liệu và lọc theo các điều kiện
        private ObservableCollection<Notification> _notifications = new ObservableCollection<Notification>();
        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }
        
        // 1. Từ khóa tìm kiếm
        // 2. Binding đến TextBox tìm kiếm
        // 3. Dùng để lọc thông báo theo tiêu đề hoặc nội dung
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    LoadNotificationsAsync();
                }
            }
        }
        
        // 1. Loại đối tượng nhận được chọn để lọc
        // 2. Binding đến ComboBox lọc theo loại đối tượng nhận
        // 3. Cho phép lọc thông báo theo loại đối tượng nhận
        private string _selectedRecipientType = "All";
        public string SelectedRecipientType
        {
            get => _selectedRecipientType;
            set
            {
                if (SetProperty(ref _selectedRecipientType, value))
                {
                    LoadNotificationsAsync();
                }
            }
        }
        
        // 1. Cờ đánh dấu có hiển thị thông báo đã hết hạn hay không
        // 2. Binding đến CheckBox hiển thị thông báo đã hết hạn
        // 3. Ảnh hưởng đến câu truy vấn lấy dữ liệu
        private bool _showExpired;
        public bool ShowExpired
        {
            get => _showExpired;
            set
            {
                if (SetProperty(ref _showExpired, value))
                {
                    LoadNotificationsAsync();
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
        
        // 1. Thông báo được chọn trong danh sách
        // 2. Binding đến SelectedItem của DataGrid
        // 3. Được sử dụng cho các thao tác sửa và xóa
        private Notification _selectedNotification;
        public Notification SelectedNotification
        {
            get => _selectedNotification;
            set => SetProperty(ref _selectedNotification, value);
        }

        // 1. Danh sách loại đối tượng nhận có sẵn để lọc
        // 2. Binding đến ComboBox lọc theo loại đối tượng nhận
        // 3. Bao gồm tất cả loại đối tượng nhận và lựa chọn "All"
        public ObservableCollection<string> RecipientTypes { get; } = new ObservableCollection<string>
        {
            "All",
            "All Users",
            "All Students",
            "All Teachers",
            "All Admins",
            "Class",
            "Student",
            "Teacher"
        };

        // 1. Lệnh thêm thông báo mới
        // 2. Binding đến nút "Thêm thông báo" trong UI
        // 3. Khi được gọi, mở hộp thoại thêm thông báo
        public ICommand AddNotificationCommand { get; }
        
        // 1. Lệnh sửa thông báo
        // 2. Binding đến nút "Sửa" trong UI
        // 3. Khi được gọi, mở hộp thoại sửa thông báo với thông tin đã chọn
        public ICommand EditNotificationCommand { get; }
        
        // 1. Lệnh xóa thông báo
        // 2. Binding đến nút "Xóa" trong UI
        // 3. Khi được gọi, hiển thị xác nhận và xóa thông báo đã chọn
        public ICommand DeleteNotificationCommand { get; }
        
        // 1. Lệnh quay lại
        // 2. Binding đến nút "Quay lại" trong UI
        // 3. Khi được gọi, chuyển về màn hình dashboard
        public ICommand BackCommand { get; }

        // 1. Constructor của lớp
        // 2. Khởi tạo các tham số và thiết lập lệnh
        // 3. Tải dữ liệu ban đầu từ cơ sở dữ liệu
        public NotificationManagementViewModel(DatabaseService databaseService, AuthenticationService authService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _authService = authService;
            _navigationService = navigationService;

            AddNotificationCommand = new RelayCommand(param => OpenAddNotificationDialog());
            EditNotificationCommand = new RelayCommand(param => OpenEditNotificationDialog(param as Notification), param => param != null);
            DeleteNotificationCommand = new RelayCommand(async param => await DeleteNotificationAsync(param as Notification), param => param != null);
            BackCommand = new RelayCommand(param => _navigationService.NavigateTo(AppViews.AdminDashboard));

            // Load initial data
            LoadNotificationsAsync();
        }

        // 1. Phương thức tải danh sách thông báo
        // 2. Truy vấn cơ sở dữ liệu với các điều kiện lọc
        // 3. Áp dụng bộ lọc theo tìm kiếm, loại đối tượng nhận và trạng thái hết hạn
        private async void LoadNotificationsAsync()
        {
            IsLoading = true;
            Notifications.Clear();

            try
            {
                string query = @"
                    SELECT 
                        n.NotificationID, n.Title, n.Message, 
                        n.SenderID, u.Username as SenderName,
                        n.RecipientType, n.RecipientID, 
                        CASE 
                            WHEN n.RecipientType = 'Class' AND n.RecipientID IS NOT NULL THEN c.ClassName
                            WHEN n.RecipientType = 'Student' AND n.RecipientID IS NOT NULL THEN CONCAT(s.FirstName, ' ', s.LastName)
                            WHEN n.RecipientType = 'Teacher' AND n.RecipientID IS NOT NULL THEN CONCAT(t.FirstName, ' ', t.LastName)
                            ELSE n.RecipientType
                        END as RecipientName,
                        n.IsRead, n.CreatedDate, n.ExpiryDate
                    FROM Notifications n
                    LEFT JOIN Users u ON n.SenderID = u.UserID
                    LEFT JOIN Classes c ON n.RecipientType = 'Class' AND n.RecipientID = c.ClassID
                    LEFT JOIN Students s ON n.RecipientType = 'Student' AND n.RecipientID = s.StudentID
                    LEFT JOIN Teachers t ON n.RecipientType = 'Teacher' AND n.RecipientID = t.TeacherID
                    WHERE 1=1";

                var parameters = new Dictionary<string, object>();

                // Apply expiry filter
                if (!ShowExpired)
                {
                    query += " AND (n.ExpiryDate IS NULL OR n.ExpiryDate > GETDATE())";
                }

                // Apply recipient type filter
                if (SelectedRecipientType != "All")
                {
                    if (SelectedRecipientType == "All Users")
                    {
                        query += " AND n.RecipientType = 'All'";
                    }
                    else if (SelectedRecipientType.StartsWith("All "))
                    {
                        // For "All Students", "All Teachers", "All Admins"
                        string type = SelectedRecipientType.Substring(4, SelectedRecipientType.Length - 5); // Remove "All " and "s"
                        query += " AND n.RecipientType = @RecipientType AND n.RecipientID IS NULL";
                        parameters.Add("@RecipientType", type);
                    }
                    else
                    {
                        // For specific recipient types (Class, Student, Teacher)
                        query += " AND n.RecipientType = @RecipientType";
                        parameters.Add("@RecipientType", SelectedRecipientType);
                    }
                }

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query += " AND (n.Title LIKE @SearchText OR n.Message LIKE @SearchText)";
                    parameters.Add("@SearchText", $"%{SearchText}%");
                }

                // Order by (newest first)
                query += " ORDER BY n.CreatedDate DESC";

                DataTable result = await _databaseService.ExecuteQueryAsync(query, parameters);

                foreach (DataRow row in result.Rows)
                {
                    DateTime createdDate = Convert.ToDateTime(row["CreatedDate"]);
                    DateTime? expiryDate = row["ExpiryDate"] != DBNull.Value ? 
                        Convert.ToDateTime(row["ExpiryDate"]) : null;

                    Notifications.Add(new Notification
                    {
                        NotificationID = Convert.ToInt32(row["NotificationID"]),
                        Title = row["Title"].ToString(),
                        Message = row["Message"].ToString(),
                        SenderID = row["SenderID"] != DBNull.Value ? Convert.ToInt32(row["SenderID"]) : null,
                        SenderName = row["SenderName"].ToString(),
                        RecipientType = row["RecipientType"].ToString(),
                        RecipientID = row["RecipientID"] != DBNull.Value ? Convert.ToInt32(row["RecipientID"]) : null,
                        RecipientName = row["RecipientName"].ToString(),
                        IsRead = Convert.ToBoolean(row["IsRead"]),
                        CreatedDate = createdDate,
                        ExpiryDate = expiryDate
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading notifications: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // 1. Phương thức mở hộp thoại thêm thông báo mới
        // 2. Tạo và hiển thị AddEditNotificationView với AddEditNotificationViewModel
        // 3. Tải lại danh sách thông báo nếu thêm thành công
        private void OpenAddNotificationDialog()
        {
            var viewModel = new AddEditNotificationViewModel(_databaseService, null, _authService.CurrentUser);
            var dialog = new AddEditNotificationView(viewModel)
            {
                Owner = Application.Current.MainWindow
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                LoadNotificationsAsync();
            }
        }

        // 1. Phương thức mở hộp thoại sửa thông báo
        // 2. Tạo và hiển thị AddEditNotificationView với AddEditNotificationViewModel và dữ liệu thông báo
        // 3. Tải lại danh sách thông báo nếu cập nhật thành công
        private void OpenEditNotificationDialog(Notification notification)
        {
            if (notification == null) return;

            var viewModel = new AddEditNotificationViewModel(
                _databaseService, 
                null, 
                _authService.CurrentUser, 
                notification);
                
            var dialog = new AddEditNotificationView(viewModel)
            {
                Owner = Application.Current.MainWindow
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                LoadNotificationsAsync();
            }
        }

        // 1. Phương thức xóa thông báo
        // 2. Hiển thị xác nhận xóa và thực hiện xóa nếu được xác nhận
        // 3. Tải lại danh sách thông báo sau khi xóa thành công
        private async Task DeleteNotificationAsync(Notification notification)
        {
            if (notification == null) return;

            // Confirm deletion
            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the notification '{notification.Title}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    IsLoading = true;

                    string query = "DELETE FROM Notifications WHERE NotificationID = @NotificationID";
                    var parameters = new Dictionary<string, object>
                    {
                        { "@NotificationID", notification.NotificationID }
                    };

                    await _databaseService.ExecuteNonQueryAsync(query, parameters);
                    
                    // Reload the notifications list
                    LoadNotificationsAsync();
                    
                    MessageBox.Show("Notification deleted successfully", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting notification: {ex.Message}", "Error", 
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
