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
    // Lớp NotificationManagementViewModel
    // + Tại sao cần sử dụng: Quản lý danh sách, tìm kiếm, lọc, xem chi tiết và xóa thông báo
    // + Lớp này được gọi từ màn hình quản lý thông báo
    // + Chức năng chính: Hiển thị và tương tác với danh sách thông báo trong hệ thống
    public class NotificationManagementViewModel : ViewModelBase
    {
        // 1. Dịch vụ cơ sở dữ liệu để truy vấn và cập nhật thông báo
        // 2. Được truyền vào từ constructor
        // 3. Dùng để tải và thao tác với dữ liệu thông báo
        private readonly DatabaseService _databaseService;
        
        // 1. Dịch vụ điều hướng
        // 2. Được truyền vào từ constructor
        // 3. Sử dụng để điều hướng giữa các màn hình
        private readonly NavigationService _navigationService;
        
        // 1. Dịch vụ xác thực
        // 2. Được truyền vào từ constructor
        // 3. Cung cấp thông tin về người dùng hiện tại
        private readonly AuthenticationService _authService;

        // 1. Danh sách thông báo hiển thị trong UI
        // 2. Binding đến ListView hoặc DataGrid
        // 3. Được tải từ cơ sở dữ liệu và lọc theo các điều kiện
        private ObservableCollection<Notification> _notifications = [];
        
        // 1. Từ khóa tìm kiếm
        // 2. Binding đến TextBox tìm kiếm
        // 3. Dùng để lọc thông báo theo tiêu đề hoặc nội dung
        private string _searchText = string.Empty;
        
        // 1. Tùy chọn hiển thị thông báo đã hết hạn
        // 2. Binding đến CheckBox trong UI
        // 3. Dùng để lọc thông báo theo trạng thái hết hạn
        private bool _showExpired = false;
        
        // 1. Trạng thái đang tải dữ liệu
        // 2. Binding đến indicator trong UI
        // 3. Cập nhật khi bắt đầu và kết thúc tải dữ liệu
        private bool _isLoading;
        
        // 1. Thông báo được chọn trong danh sách
        // 2. Binding đến SelectedItem của ListView/DataGrid
        // 3. Dùng cho các thao tác xem chi tiết, sửa, xóa
        private Notification? _selectedNotification;
        
        // 1. Loại người nhận được chọn để lọc
        // 2. Binding đến ComboBox lọc theo loại người nhận
        // 3. Dùng để lọc thông báo theo loại người nhận
        private string _selectedRecipientTypeFilter = "All";
        
        // 1. Danh sách loại người nhận để lọc
        // 2. Binding đến ItemsSource của ComboBox lọc
        // 3. Cung cấp các tùy chọn lọc theo loại người nhận
        private ObservableCollection<string> _recipientTypeFilters = new ObservableCollection<string>
        {
            "All", "All Users", "Class", "Teacher", "Student"
        };

        // 1. Danh sách thông báo hiển thị trong UI
        // 2. Binding đến ListView hoặc DataGrid
        // 3. Được tải từ cơ sở dữ liệu và lọc theo các điều kiện
        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }

        // 1. Từ khóa tìm kiếm
        // 2. Binding đến TextBox tìm kiếm
        // 3. Dùng để lọc thông báo theo tiêu đề hoặc nội dung
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = RefreshNotificationsAsync();
                }
            }
        }

        // 1. Loại người nhận được chọn để lọc
        // 2. Binding đến ComboBox lọc theo loại người nhận
        // 3. Dùng để lọc thông báo theo loại người nhận
        public string SelectedRecipientTypeFilter
        {
            get => _selectedRecipientTypeFilter;
            set
            {
                if (SetProperty(ref _selectedRecipientTypeFilter, value))
                {
                    _ = RefreshNotificationsAsync();
                }
            }
        }

        // 1. Danh sách loại người nhận để lọc
        // 2. Binding đến ItemsSource của ComboBox lọc
        // 3. Cung cấp các tùy chọn lọc theo loại người nhận
        public ObservableCollection<string> RecipientTypeFilters
        {
            get => _recipientTypeFilters;
            set => SetProperty(ref _recipientTypeFilters, value);
        }

        // 1. Tùy chọn hiển thị thông báo đã hết hạn
        // 2. Binding đến CheckBox trong UI
        // 3. Dùng để lọc thông báo theo trạng thái hết hạn
        public bool ShowExpired
        {
            get => _showExpired;
            set
            {
                if (SetProperty(ref _showExpired, value))
                {
                    _ = RefreshNotificationsAsync();
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

        // 1. Thông báo được chọn trong danh sách
        // 2. Binding đến SelectedItem của ListView/DataGrid
        // 3. Dùng cho các thao tác xem chi tiết, sửa, xóa
        public Notification? SelectedNotification
        {
            get => _selectedNotification;
            set => SetProperty(ref _selectedNotification, value);
        }

        // 1. Lệnh tạo thông báo mới
        // 2. Binding đến nút tạo thông báo trong UI
        // 3. Mở hộp thoại tạo thông báo mới
        public ICommand CreateNotificationCommand { get; }
        
        // 1. Lệnh xem chi tiết thông báo
        // 2. Binding đến nút xem hoặc sự kiện double-click
        // 3. Hiển thị chi tiết thông báo và đánh dấu đã đọc
        public ICommand ViewNotificationCommand { get; }
        
        // 1. Lệnh sửa thông báo
        // 2. Binding đến nút sửa trong UI
        // 3. Mở hộp thoại sửa thông báo
        public ICommand EditNotificationCommand { get; }
        
        // 1. Lệnh xóa thông báo
        // 2. Binding đến nút xóa trong UI
        // 3. Xóa thông báo đã chọn sau khi xác nhận
        public ICommand DeleteNotificationCommand { get; }
        
        // 1. Lệnh quay lại màn hình trước
        // 2. Binding đến nút quay lại trong UI
        // 3. Điều hướng về màn hình dashboard
        public ICommand BackCommand { get; }

        // 1. Constructor của lớp
        // 2. Khởi tạo các dịch vụ và lệnh
        // 3. Tải dữ liệu ban đầu
        public NotificationManagementViewModel(DatabaseService databaseService, AuthenticationService authService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _authService = authService;
            _navigationService = navigationService;

            CreateNotificationCommand = new RelayCommand(_ => CreateNotification());
            ViewNotificationCommand = new RelayCommand(param => ViewNotification(param as Notification), param => param != null);
            EditNotificationCommand = new RelayCommand(param => EditNotification(param as Notification), param => param != null);
            DeleteNotificationCommand = new RelayCommand(async param => await DeleteNotificationAsync(param as Notification), param => param != null);
            BackCommand = new RelayCommand(_ => _navigationService.NavigateTo(AppViews.AdminDashboard));
            
            // Load notifications when ViewModel is created
            _ = LoadNotificationsAsync();
        }
        
        // 1. Phương thức làm mới danh sách thông báo
        // 2. Gọi khi có thay đổi ở các bộ lọc
        // 3. Tải lại dữ liệu từ cơ sở dữ liệu
        private async Task RefreshNotificationsAsync()
        {
            await LoadNotificationsAsync();
        }
        
        // 1. Phương thức tải danh sách thông báo
        // 2. Truy vấn cơ sở dữ liệu với các điều kiện lọc
        // 3. Điền dữ liệu vào danh sách Notifications
        private async Task LoadNotificationsAsync()
        {
            try
            {
                IsLoading = true;
                Notifications.Clear();

                string query = BuildNotificationQuery();
                var parameters = BuildQueryParameters();
                DataTable result = await _databaseService.ExecuteQueryAsync(query, parameters);
                
                PopulateNotificationsFromDataTable(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading notifications: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        // 1. Phương thức xây dựng câu truy vấn SQL
        // 2. Tạo câu truy vấn dựa trên các điều kiện lọc
        // 3. Trả về chuỗi SQL hoàn chỉnh
        private string BuildNotificationQuery()
        {
            string query = @"
                SELECT 
                    n.NotificationID, 
                    n.Title, 
                    n.Message, 
                    n.SenderID,
                    u.Username AS SenderName,
                    n.RecipientType,
                    n.RecipientID,
                    CASE 
                        WHEN n.RecipientType = 'Class' THEN (SELECT ClassName FROM Classes WHERE ClassID = n.RecipientID)
                        WHEN n.RecipientType = 'Teacher' THEN (SELECT FirstName + ' ' + LastName FROM Teachers WHERE TeacherID = n.RecipientID)
                        WHEN n.RecipientType = 'Student' THEN (SELECT FirstName + ' ' + LastName FROM Students WHERE StudentID = n.RecipientID)
                        ELSE 'All Users'
                    END AS RecipientName,
                    n.IsRead,
                    n.CreatedDate,
                    n.ExpiryDate
                FROM Notifications n
                LEFT JOIN Users u ON n.SenderID = u.UserID
                WHERE 1=1";
            
            if (!_showExpired)
            {
                query += " AND (n.ExpiryDate IS NULL OR n.ExpiryDate >= GETDATE())";
            }

            if (_selectedRecipientTypeFilter != "All")
            {
                if (_selectedRecipientTypeFilter == "All Users")
                {
                    query += " AND n.RecipientType = 'All'";
                }
                else
                {
                    query += " AND n.RecipientType = @RecipientType";
                }
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                query += " AND (n.Title LIKE @Search OR n.Message LIKE @Search)";
            }

            query += " ORDER BY n.CreatedDate DESC";
            
            return query;
        }
        
        // 1. Phương thức xây dựng tham số cho truy vấn
        // 2. Tạo dictionary các tham số dựa trên điều kiện lọc
        // 3. Trả về dictionary chứa tham số và giá trị
        private Dictionary<string, object> BuildQueryParameters()
        {
            var parameters = new Dictionary<string, object>();
            
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                parameters["@Search"] = $"%{_searchText}%";
            }
            
            if (_selectedRecipientTypeFilter != "All" && _selectedRecipientTypeFilter != "All Users")
            {
                parameters["@RecipientType"] = _selectedRecipientTypeFilter;
            }
            
            return parameters;
        }
        
        // 1. Phương thức điền dữ liệu từ DataTable vào danh sách thông báo
        // 2. Chuyển đổi từng dòng dữ liệu thành đối tượng Notification
        // 3. Thêm các đối tượng vào danh sách Notifications
        private void PopulateNotificationsFromDataTable(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Notifications.Add(new Notification
                {
                    NotificationID = Convert.ToInt32(row["NotificationID"]),
                    Title = row["Title"].ToString() ?? string.Empty,
                    Message = row["Message"].ToString() ?? string.Empty,
                    SenderID = Convert.ToInt32(row["SenderID"]),
                    SenderName = row["SenderName"].ToString() ?? string.Empty,
                    RecipientType = row["RecipientType"].ToString() ?? string.Empty,
                    RecipientID = row["RecipientID"] != DBNull.Value ? Convert.ToInt32(row["RecipientID"]) : null,
                    RecipientName = row["RecipientName"].ToString() ?? string.Empty,
                    IsRead = Convert.ToBoolean(row["IsRead"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ExpiryDate = row["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(row["ExpiryDate"]) : null
                });
            }
        }
        
        // 1. Phương thức tạo thông báo mới
        // 2. Mở hộp thoại tạo thông báo
        // 3. Hiện tại chỉ hiển thị thông báo giữ chỗ
        private void CreateNotification()
        {
            // For now just show a placeholder message
            // In a future update, implement a Create Notification dialog
            MessageBox.Show("Create Notification functionality will be implemented in a future update.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        // 1. Phương thức xem chi tiết thông báo
        // 2. Hiển thị hộp thoại với thông tin chi tiết
        // 3. Đánh dấu thông báo là đã đọc nếu chưa
        private void ViewNotification(Notification? notification)
        {
            if (notification == null) return;
            
            // Show notification details in a dialog
            MessageBox.Show(
                $"Title: {notification.Title}\n\n" +
                $"Message: {notification.Message}\n\n" +
                $"Sent By: {notification.SenderName}\n" +
                $"Sent To: {notification.RecipientType} - {notification.RecipientName}\n" +
                $"Created: {notification.CreatedDate}\n" +
                $"Expires: {notification.ExpiryDate?.ToString() ?? "Never"}\n" +
                $"Status: {notification.StatusDisplay}",
                "Notification Details", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);
            
            // If it wasn't read, mark it as read
            if (!notification.IsRead)
            {
                MarkAsReadAsync(notification);
            }
        }
        
        // 1. Phương thức đánh dấu thông báo là đã đọc
        // 2. Cập nhật trạng thái trong cơ sở dữ liệu
        // 3. Cập nhật thuộc tính IsRead của đối tượng
        private async Task MarkAsReadAsync(Notification notification)
        {
            try
            {
                string query = "UPDATE Notifications SET IsRead = 1 WHERE NotificationID = @NotificationID";
                var parameters = new Dictionary<string, object>
                {
                    { "@NotificationID", notification.NotificationID }
                };
                
                await _databaseService.ExecuteNonQueryAsync(query, parameters);
                notification.IsRead = true;
                
                // No need to refresh the whole list since we just update the local object
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error marking notification as read: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // 1. Phương thức chỉnh sửa thông báo
        // 2. Mở hộp thoại chỉnh sửa thông báo
        // 3. Hiện tại chỉ hiển thị thông báo giữ chỗ
        private void EditNotification(Notification? notification)
        {
            if (notification == null) return;
            
            // For now just show a placeholder message
            // In a future update, implement an Edit Notification dialog
            MessageBox.Show($"Edit Notification '{notification.Title}' functionality will be implemented in a future update.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        // 1. Phương thức xóa thông báo
        // 2. Hiển thị hộp thoại xác nhận xóa
        // 3. Nếu xác nhận, gọi phương thức xóa từ cơ sở dữ liệu
        private async Task DeleteNotificationAsync(Notification? notification)
        {
            if (notification == null) return;
            
            // Confirm deletion
            var result = MessageBox.Show($"Are you sure you want to delete notification '{notification.Title}'?", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                await DeleteNotificationFromDatabaseAsync(notification);
            }
        }
        
        // 1. Phương thức xóa thông báo từ cơ sở dữ liệu
        // 2. Thực hiện truy vấn DELETE
        // 3. Làm mới danh sách và hiển thị thông báo kết quả
        private async Task DeleteNotificationFromDatabaseAsync(Notification notification)
        {
            try
            {
                IsLoading = true;
                
                // Delete the notification
                string query = "DELETE FROM Notifications WHERE NotificationID = @NotificationID";
                var parameters = new Dictionary<string, object>
                {
                    { "@NotificationID", notification.NotificationID }
                };
                
                await _databaseService.ExecuteNonQueryAsync(query, parameters);
                
                // Refresh the list
                await LoadNotificationsAsync();

                MessageBox.Show($"Notification '{notification.Title}' has been deleted.", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting notification: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
