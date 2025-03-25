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
    // Lớp AddEditNotificationViewModel
    // + Tại sao cần sử dụng: Cung cấp chức năng tạo mới và chỉnh sửa thông báo
    // + Lớp này được gọi từ NotificationManagementViewModel khi cần tạo hoặc sửa thông báo
    // + Chức năng chính: Quản lý giao diện, xác thực dữ liệu và lưu thông báo
    public class AddEditNotificationViewModel : ViewModelBase
    {
        // 1. Dịch vụ cơ sở dữ liệu để truy vấn và cập nhật thông báo
        // 2. Được truyền vào từ constructor
        // 3. Dùng để lưu thông báo và tải danh sách người nhận
        private readonly DatabaseService _databaseService;
        
        // 1. Dịch vụ xác thực để lấy thông tin người dùng hiện tại
        // 2. Được truyền vào từ constructor
        // 3. Dùng để xác định người gửi thông báo
        private readonly AuthenticationService _authService;
        
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
        // 3. Thay đổi theo chế độ: "Create New Notification" hoặc "Edit Notification"
        private string _dialogTitle;
        
        // 1. Thông báo lỗi khi xác thực dữ liệu
        // 2. Hiển thị khi dữ liệu không hợp lệ
        // 3. Được xóa khi dữ liệu đã được sửa
        private string _errorMessage = string.Empty;
        
        // 1. Đối tượng thông báo đang được chỉnh sửa hoặc tạo mới
        // 2. Chứa tất cả thông tin về thông báo
        // 3. Được binding đến các control trong form
        private Notification _notification;
        
        // 1. Danh sách người nhận có thể chọn
        // 2. Được tải dựa trên loại người nhận (Class, Teacher, Student)
        // 3. Hiển thị trong ComboBox để người dùng chọn
        private ObservableCollection<RecipientViewModel> _recipients = new ObservableCollection<RecipientViewModel>();
        
        // 1. Người nhận được chọn từ danh sách
        // 2. Được cập nhật khi người dùng chọn từ ComboBox
        // 3. Cập nhật thông tin RecipientID và RecipientName
        private RecipientViewModel _selectedRecipient;
        
        // 1. Loại người nhận được chọn (All, Class, Teacher, Student)
        // 2. Được cập nhật khi người dùng chọn từ ComboBox
        // 3. Ảnh hưởng đến danh sách người nhận có thể chọn
        private string _selectedRecipientType = "All";
        
        // 1. Ngày hết hạn của thông báo
        // 2. Được cập nhật khi người dùng chọn từ DatePicker
        // 3. Có thể null nếu thông báo không có ngày hết hạn
        private DateTime? _expiryDate;
        
        // 1. Xác định thông báo có ngày hết hạn hay không
        // 2. Được cập nhật khi người dùng chọn từ CheckBox
        // 3. Ảnh hưởng đến việc hiển thị và sử dụng DatePicker
        private bool _hasExpiryDate;

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

        // 1. Đối tượng thông báo đang được chỉnh sửa
        // 2. Binding đến các control trong form
        // 3. Chứa tất cả thông tin của thông báo
        public Notification Notification
        {
            get => _notification;
            set => SetProperty(ref _notification, value);
        }

        // 1. Danh sách người nhận có thể chọn
        // 2. Binding đến ComboBox người nhận
        // 3. Được tải dựa trên loại người nhận đã chọn
        public ObservableCollection<RecipientViewModel> Recipients
        {
            get => _recipients;
            set => SetProperty(ref _recipients, value);
        }

        // 1. Người nhận được chọn hiện tại
        // 2. Binding đến SelectedItem của ComboBox người nhận
        // 3. Cập nhật RecipientID và RecipientName khi thay đổi
        public RecipientViewModel SelectedRecipient
        {
            get => _selectedRecipient;
            set
            {
                if (SetProperty(ref _selectedRecipient, value) && value != null)
                {
                    Notification.RecipientID = value.ID;
                    Notification.RecipientName = value.Name;
                }
            }
        }

        // 1. Loại người nhận được chọn hiện tại
        // 2. Binding đến SelectedItem của ComboBox loại người nhận
        // 3. Khi thay đổi, tải lại danh sách người nhận tương ứng
        public string SelectedRecipientType
        {
            get => _selectedRecipientType;
            set
            {
                if (SetProperty(ref _selectedRecipientType, value))
                {
                    Notification.RecipientType = value;
                    Notification.RecipientID = null;
                    Notification.RecipientName = "All Users";
                    
                    // Tải danh sách người nhận dựa trên loại người nhận
                    LoadRecipientsAsync();
                }
            }
        }

        // 1. Ngày hết hạn của thông báo
        // 2. Binding đến DatePicker chọn ngày hết hạn
        // 3. Được cập nhật vào Notification khi thay đổi
        public DateTime? ExpiryDate
        {
            get => _expiryDate;
            set
            {
                if (SetProperty(ref _expiryDate, value))
                {
                    Notification.ExpiryDate = value;
                }
            }
        }

        // 1. Xác định thông báo có ngày hết hạn hay không
        // 2. Binding đến CheckBox "Has Expiry Date"
        // 3. Khi chọn, hiển thị DatePicker và thiết lập ngày mặc định
        public bool HasExpiryDate
        {
            get => _hasExpiryDate;
            set
            {
                if (SetProperty(ref _hasExpiryDate, value))
                {
                    ExpiryDate = value ? DateTime.Now.AddDays(7) : null;
                }
            }
        }

        // 1. Danh sách các loại người nhận có thể chọn
        // 2. Binding đến ItemsSource của ComboBox loại người nhận
        // 3. Danh sách cố định các loại người nhận trong hệ thống
        public ObservableCollection<string> RecipientTypes { get; } = new ObservableCollection<string>
        {
            "All", "Class", "Teacher", "Student"
        };

        // 1. Lệnh lưu thông báo
        // 2. Binding đến nút "Save" trong form
        // 3. Gọi phương thức SaveNotification khi được kích hoạt
        public ICommand SaveCommand { get; }
        
        // 1. Lệnh hủy bỏ và đóng form
        // 2. Binding đến nút "Cancel" trong form
        // 3. Gọi phương thức CloseDialog khi được kích hoạt
        public ICommand CancelCommand { get; }

        // 1. Constructor cho chế độ thêm mới thông báo
        // 2. Khởi tạo ViewModel với các tham số cần thiết
        // 3. Tạo thông báo mới với thông tin người gửi từ người dùng hiện tại
        public AddEditNotificationViewModel(DatabaseService databaseService, AuthenticationService authService, Window dialogWindow)
        {
            _databaseService = databaseService;
            _authService = authService;
            _dialogWindow = dialogWindow;
            _isEditMode = false;
            DialogTitle = "Create New Notification";

            // Khởi tạo đối tượng thông báo mới
            Notification = new Notification
            {
                SenderID = _authService.CurrentUser?.UserID ?? 0,
                SenderName = _authService.CurrentUser?.Username ?? "System",
                RecipientType = "All",
                IsRead = false,
                CreatedDate = DateTime.Now
            };

            // Khởi tạo các lệnh
            SaveCommand = new RelayCommand(async param => await SaveNotification(), param => CanSaveNotification());
            CancelCommand = new RelayCommand(param => CloseDialog());

            // Tải danh sách người nhận
            LoadRecipientsAsync();
        }

        // 1. Constructor cho chế độ chỉnh sửa thông báo
        // 2. Khởi tạo ViewModel với thông báo cần chỉnh sửa
        // 3. Sao chép thông báo để tránh thay đổi trực tiếp
        public AddEditNotificationViewModel(DatabaseService databaseService, AuthenticationService authService, Window dialogWindow, Notification notificationToEdit)
        {
            _databaseService = databaseService;
            _authService = authService;
            _dialogWindow = dialogWindow;
            _isEditMode = true;
            DialogTitle = "Edit Notification";

            // Tạo bản sao của thông báo cần chỉnh sửa
            Notification = new Notification
            {
                NotificationID = notificationToEdit.NotificationID,
                Title = notificationToEdit.Title,
                Message = notificationToEdit.Message,
                SenderID = notificationToEdit.SenderID,
                SenderName = notificationToEdit.SenderName,
                RecipientType = notificationToEdit.RecipientType,
                RecipientID = notificationToEdit.RecipientID,
                RecipientName = notificationToEdit.RecipientName,
                IsRead = notificationToEdit.IsRead,
                CreatedDate = notificationToEdit.CreatedDate,
                ExpiryDate = notificationToEdit.ExpiryDate
            };

            // Thiết lập các thuộc tính ban đầu
            _selectedRecipientType = notificationToEdit.RecipientType;
            _expiryDate = notificationToEdit.ExpiryDate;
            _hasExpiryDate = notificationToEdit.ExpiryDate.HasValue;

            // Khởi tạo các lệnh
            SaveCommand = new RelayCommand(async param => await SaveNotification(), param => CanSaveNotification());
            CancelCommand = new RelayCommand(param => CloseDialog());

            // Tải danh sách người nhận
            LoadRecipientsAsync();
        }

        // 1. Phương thức tải danh sách người nhận dựa trên loại đã chọn
        // 2. Truy vấn cơ sở dữ liệu để lấy danh sách tương ứng
        // 3. Điền vào danh sách và thiết lập người nhận đã chọn
        private async void LoadRecipientsAsync()
        {
            try
            {
                Recipients.Clear();

                // Nếu loại người nhận là "All", không cần tải danh sách
                if (_selectedRecipientType == "All")
                {
                    Recipients.Add(new RecipientViewModel { ID = null, Name = "All Users" });
                    SelectedRecipient = Recipients[0];
                    return;
                }

                string query = "";
                
                // Xây dựng truy vấn dựa trên loại người nhận
                switch (_selectedRecipientType)
                {
                    case "Class":
                        query = "SELECT ClassID AS ID, ClassName AS Name FROM Classes WHERE IsActive = 1 ORDER BY ClassName";
                        break;
                    case "Teacher":
                        query = "SELECT TeacherID AS ID, FirstName + ' ' + LastName AS Name FROM Teachers ORDER BY LastName, FirstName";
                        break;
                    case "Student":
                        query = "SELECT StudentID AS ID, FirstName + ' ' + LastName AS Name FROM Students ORDER BY LastName, FirstName";
                        break;
                }

                // Thực thi truy vấn
                var result = await _databaseService.ExecuteQueryAsync(query);

                // Thêm đối tượng đại diện cho "Select a recipient"
                Recipients.Add(new RecipientViewModel { ID = null, Name = $"-- Select a {_selectedRecipientType} --" });

                // Điền dữ liệu vào danh sách
                foreach (DataRow row in result.Rows)
                {
                    Recipients.Add(new RecipientViewModel
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        Name = row["Name"].ToString() ?? string.Empty
                    });
                }

                // Đặt đối tượng được chọn
                if (_isEditMode && Notification.RecipientID.HasValue)
                {
                    foreach (var recipient in Recipients)
                    {
                        if (recipient.ID == Notification.RecipientID)
                        {
                            SelectedRecipient = recipient;
                            break;
                        }
                    }
                }
                else
                {
                    // Mặc định là đối tượng đầu tiên
                    SelectedRecipient = Recipients[0];
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                MessageBox.Show($"Error loading recipients: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 1. Phương thức kiểm tra thông báo có thể lưu không
        // 2. Xác thực các trường bắt buộc đã được nhập
        // 3. Dùng để kích hoạt/vô hiệu hóa nút Save
        private bool CanSaveNotification()
        {
            return !string.IsNullOrWhiteSpace(Notification.Title) && 
                   !string.IsNullOrWhiteSpace(Notification.Message) &&
                   (Notification.RecipientType == "All" || Notification.RecipientID.HasValue);
        }

        // 1. Phương thức lưu thông báo vào cơ sở dữ liệu
        // 2. Xác thực dữ liệu và thực hiện lưu hoặc cập nhật
        // 3. Hiển thị thông báo kết quả và đóng dialog
        private async Task SaveNotification()
        {
            try
            {
                ErrorMessage = string.Empty;

                // Kiểm tra dữ liệu nhập vào
                if (string.IsNullOrWhiteSpace(Notification.Title))
                {
                    ErrorMessage = "Title is required.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Notification.Message))
                {
                    ErrorMessage = "Message is required.";
                    return;
                }

                if (Notification.RecipientType != "All" && !Notification.RecipientID.HasValue)
                {
                    ErrorMessage = $"Please select a {Notification.RecipientType}.";
                    return;
                }

                if (_isEditMode)
                {
                    // Cập nhật thông báo hiện có
                    string query = @"
                        UPDATE Notifications 
                        SET Title = @Title, 
                            Message = @Message, 
                            RecipientType = @RecipientType, 
                            RecipientID = @RecipientID, 
                            ExpiryDate = @ExpiryDate
                        WHERE NotificationID = @NotificationID";

                    var parameters = new Dictionary<string, object>
                    {
                        { "@NotificationID", Notification.NotificationID },
                        { "@Title", Notification.Title },
                        { "@Message", Notification.Message },
                        { "@RecipientType", Notification.RecipientType },
                        { "@RecipientID", Notification.RecipientID.HasValue ? (object)Notification.RecipientID : DBNull.Value },
                        { "@ExpiryDate", Notification.ExpiryDate.HasValue ? (object)Notification.ExpiryDate : DBNull.Value }
                    };

                    await _databaseService.ExecuteNonQueryAsync(query, parameters);
                    MessageBox.Show("Notification updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Thêm thông báo mới
                    string query = @"
                        INSERT INTO Notifications (
                            Title, Message, SenderID, RecipientType, RecipientID, 
                            IsRead, CreatedDate, ExpiryDate
                        ) VALUES (
                            @Title, @Message, @SenderID, @RecipientType, @RecipientID, 
                            @IsRead, @CreatedDate, @ExpiryDate
                        );
                        SELECT SCOPE_IDENTITY();";

                    var parameters = new Dictionary<string, object>
                    {
                        { "@Title", Notification.Title },
                        { "@Message", Notification.Message },
                        { "@SenderID", Notification.SenderID },
                        { "@RecipientType", Notification.RecipientType },
                        { "@RecipientID", Notification.RecipientID.HasValue ? (object)Notification.RecipientID : DBNull.Value },
                        { "@IsRead", false },
                        { "@CreatedDate", DateTime.Now },
                        { "@ExpiryDate", Notification.ExpiryDate.HasValue ? (object)Notification.ExpiryDate : DBNull.Value }
                    };

                    var notificationId = await _databaseService.ExecuteScalarAsync(query, parameters);
                    MessageBox.Show("Notification created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Đóng hộp thoại với kết quả OK
                _dialogWindow.DialogResult = true;
                _dialogWindow.Close();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                ErrorMessage = $"Error saving notification: {ex.Message}";
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

    // Lớp hỗ trợ cho người nhận
    // + Tại sao cần sử dụng: Cung cấp cấu trúc dữ liệu đơn giản cho đối tượng người nhận
    // + Lớp này được sử dụng trong danh sách Recipients và không liên kết với cơ sở dữ liệu
    // + Chức năng chính: Lưu trữ ID và tên của người nhận để hiển thị trong ComboBox
    public class RecipientViewModel
    {
        // 1. ID của người nhận (có thể là ClassID, TeacherID, StudentID)
        // 2. Có thể null nếu là "All Users" hoặc lựa chọn mặc định
        // 3. Được sử dụng để cập nhật RecipientID của thông báo
        public int? ID { get; set; }
        
        // 1. Tên hiển thị của người nhận
        // 2. Được hiển thị trong ComboBox
        // 3. Được sử dụng để cập nhật RecipientName của thông báo
        public string Name { get; set; } = string.Empty;
    }
}
