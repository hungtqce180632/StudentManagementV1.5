using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Models;
using StudentManagementV1._5.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp AddEditNotificationViewModel
    // + Tại sao cần sử dụng: Xử lý logic cho việc thêm mới hoặc chỉnh sửa thông báo
    // + Lớp này được gọi từ NotificationManagementViewModel khi mở dialog thêm/sửa thông báo
    // + Chức năng chính: Xác thực dữ liệu, xử lý lưu và cập nhật thông tin thông báo
    public class AddEditNotificationViewModel : ViewModelBase
    {
        // 1. Dịch vụ cơ sở dữ liệu để truy vấn và cập nhật thông tin thông báo
        // 2. Được truyền vào từ constructor
        // 3. Dùng để tải và thao tác với dữ liệu thông báo
        private readonly DatabaseService _databaseService;
        
        // 1. Cửa sổ dialog hiện tại
        // 2. Được truyền vào từ constructor
        // 3. Dùng để đóng dialog khi hoàn thành
        private readonly Window _dialogWindow;
        
        // 1. Người dùng hiện tại đang đăng nhập
        // 2. Được sử dụng làm SenderID cho thông báo
        // 3. Được truyền vào từ NotificationManagementViewModel
        private readonly User _currentUser;
        
        // 1. Thông báo hiện tại đang được chỉnh sửa
        // 2. Null nếu đang thêm mới thông báo
        // 3. Được truyền vào từ NotificationManagementViewModel
        private Notification _currentNotification;
        
        // 1. Sự kiện yêu cầu đóng dialog
        // 2. Được gọi khi người dùng lưu thành công hoặc hủy
        // 3. Truyền kết quả về cho View thông qua DialogResult
        public event Action<bool> RequestClose;
        
        // 1. Cờ đánh dấu đây là chế độ chỉnh sửa hay thêm mới
        // 2. True nếu đang chỉnh sửa thông báo hiện có
        // 3. False nếu đang thêm mới thông báo
        private bool _isEditMode;
        public bool IsEditMode 
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }
        
        // 1. Tiêu đề của dialog
        // 2. Thay đổi dựa trên việc thêm mới hay chỉnh sửa
        // 3. Hiển thị trên thanh tiêu đề của dialog
        private string _dialogTitle = "Add New Notification";
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
        
        // 1. Tiêu đề của thông báo
        // 2. Bắt buộc và hiển thị cho người dùng
        // 3. Cần ngắn gọn và mô tả được nội dung
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                if (SetProperty(ref _title, value))
                {
                    ValidateTitle();
                }
            }
        }
        
        // 1. Lỗi tiêu đề
        // 2. Hiển thị khi tiêu đề không hợp lệ
        // 3. Rỗng khi không có lỗi
        private string _titleError = string.Empty;
        public string TitleError
        {
            get => _titleError;
            set => SetProperty(ref _titleError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi tiêu đề hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasTitleError => !string.IsNullOrEmpty(TitleError);
        
        // 1. Nội dung của thông báo
        // 2. Bắt buộc và hiển thị cho người dùng
        // 3. Chứa thông tin chi tiết của thông báo
        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set
            {
                if (SetProperty(ref _message, value))
                {
                    ValidateMessage();
                }
            }
        }
        
        // 1. Lỗi nội dung
        // 2. Hiển thị khi nội dung không hợp lệ
        // 3. Rỗng khi không có lỗi
        private string _messageError = string.Empty;
        public string MessageError
        {
            get => _messageError;
            set => SetProperty(ref _messageError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi nội dung hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasMessageError => !string.IsNullOrEmpty(MessageError);
        
        // 1. Danh sách các loại đối tượng nhận có thể chọn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Bao gồm các loại đối tượng trong hệ thống
        public ObservableCollection<string> RecipientTypes { get; } = new ObservableCollection<string>
        {
            "All Users", 
            "All Students",
            "All Teachers",
            "All Admins",
            "Specific Class",
            "Specific Student",
            "Specific Teacher"
        };
        
        // 1. Loại đối tượng nhận được chọn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Ảnh hưởng đến danh sách người nhận cụ thể
        private string _selectedRecipientType;
        public string SelectedRecipientType
        {
            get => _selectedRecipientType;
            set
            {
                if (SetProperty(ref _selectedRecipientType, value))
                {
                    ValidateRecipientType();
                    OnPropertyChanged(nameof(ShowSpecificRecipient));
                    LoadRecipientsAsync();
                }
            }
        }
        
        // 1. Lỗi loại đối tượng nhận
        // 2. Hiển thị khi chưa chọn loại đối tượng nhận
        // 3. Rỗng khi không có lỗi
        private string _recipientTypeError = string.Empty;
        public string RecipientTypeError
        {
            get => _recipientTypeError;
            set => SetProperty(ref _recipientTypeError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi loại đối tượng nhận hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasRecipientTypeError => !string.IsNullOrEmpty(RecipientTypeError);
        
        // 1. Cờ đánh dấu có hiển thị chọn người nhận cụ thể hay không
        // 2. Dựa vào loại đối tượng nhận được chọn
        // 3. True nếu cần chọn đối tượng cụ thể, False nếu gửi cho tất cả
        public bool ShowSpecificRecipient => 
            SelectedRecipientType == "Specific Class" || 
            SelectedRecipientType == "Specific Student" || 
            SelectedRecipientType == "Specific Teacher";
        
        // 1. Danh sách đối tượng nhận cụ thể
        // 2. Binding đến ComboBox trong giao diện
        // 3. Thay đổi dựa trên loại đối tượng nhận được chọn
        private ObservableCollection<RecipientItem> _recipients = new ObservableCollection<RecipientItem>();
        public ObservableCollection<RecipientItem> Recipients
        {
            get => _recipients;
            set => SetProperty(ref _recipients, value);
        }
        
        // 1. Đối tượng nhận cụ thể được chọn
        // 2. Binding đến ComboBox trong giao diện
        // 3. Dùng để lấy RecipientID khi lưu thông báo
        private RecipientItem _selectedRecipient;
        public RecipientItem SelectedRecipient
        {
            get => _selectedRecipient;
            set => SetProperty(ref _selectedRecipient, value);
        }
        
        // 1. Ngày hết hạn của thông báo
        // 2. Binding đến DatePicker trong giao diện
        // 3. Null nếu thông báo không có thời hạn
        private DateTime? _expiryDate;
        public DateTime? ExpiryDate
        {
            get => _expiryDate;
            set
            {
                if (SetProperty(ref _expiryDate, value))
                {
                    ValidateExpiryDate();
                }
            }
        }
        
        // 1. Thời gian hết hạn của thông báo
        // 2. Binding đến TextBox trong giao diện
        // 3. Định dạng HH:mm, mặc định là 23:59
        private string _expiryTime = "23:59";
        public string ExpiryTime
        {
            get => _expiryTime;
            set
            {
                if (SetProperty(ref _expiryTime, value))
                {
                    ValidateExpiryDate();
                }
            }
        }
        
        // 1. Lỗi ngày hết hạn
        // 2. Hiển thị khi ngày hết hạn không hợp lệ
        // 3. Rỗng khi không có lỗi
        private string _expiryDateError = string.Empty;
        public string ExpiryDateError
        {
            get => _expiryDateError;
            set => SetProperty(ref _expiryDateError, value);
        }
        
        // 1. Cờ đánh dấu có lỗi ngày hết hạn hay không
        // 2. Dùng để hiển thị thông báo lỗi
        // 3. True khi có lỗi, False khi không có lỗi
        public bool HasExpiryDateError => !string.IsNullOrEmpty(ExpiryDateError);
        
        // 1. Trạng thái đã đọc của thông báo
        // 2. Chỉ hiển thị khi đang chỉnh sửa thông báo
        // 3. Binding đến CheckBox trong giao diện
        private bool _isRead;
        public bool IsRead
        {
            get => _isRead;
            set => SetProperty(ref _isRead, value);
        }
        
        // 1. Lệnh lưu thông báo
        // 2. Binding đến nút "Lưu" trong giao diện
        // 3. Thực hiện xác thực và lưu dữ liệu
        public ICommand SaveCommand { get; }
        
        // 1. Lệnh hủy thao tác
        // 2. Binding đến nút "Hủy" trong giao diện
        // 3. Đóng dialog mà không lưu
        public ICommand CancelCommand { get; }

        // 1. Constructor cho thông báo mới
        // 2. Khởi tạo ViewModel để thêm thông báo mới
        // 3. Thiết lập tiêu đề và trạng thái phù hợp
        public AddEditNotificationViewModel(DatabaseService databaseService, Window dialogWindow, User currentUser)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _currentUser = currentUser;
            _isEditMode = false;
            DialogTitle = "Add New Notification";
            
            SaveCommand = new RelayCommand(async param => await SaveNotificationAsync(), param => CanSaveNotification());
            CancelCommand = new RelayCommand(param => CloseDialog(false));
            
            // Set default values
            ExpiryDate = DateTime.Now.AddDays(7);
            SelectedRecipientType = RecipientTypes[0]; // Default to "All Users"
        }
        
        // 1. Constructor cho chỉnh sửa thông báo
        // 2. Khởi tạo ViewModel với thông tin thông báo hiện có
        // 3. Điền dữ liệu từ thông báo được chỉnh sửa
        public AddEditNotificationViewModel(DatabaseService databaseService, Window dialogWindow, User currentUser, Notification notification)
        {
            _databaseService = databaseService;
            _dialogWindow = dialogWindow;
            _currentUser = currentUser;
            _currentNotification = notification;
            _isEditMode = true;
            DialogTitle = "Edit Notification";
            
            SaveCommand = new RelayCommand(async param => await SaveNotificationAsync(), param => CanSaveNotification());
            CancelCommand = new RelayCommand(param => CloseDialog(false));
            
            // Populate fields from existing notification
            Title = notification.Title;
            Message = notification.Message;
            IsRead = notification.IsRead;
            
            // Set expiry date and time
            if (notification.ExpiryDate.HasValue)
            {
                ExpiryDate = notification.ExpiryDate.Value.Date;
                ExpiryTime = notification.ExpiryDate.Value.ToString("HH:mm");
            }
            
            // Set recipient type
            SetRecipientTypeFromNotification(notification);
        }
        
        // 1. Phương thức thiết lập loại đối tượng nhận từ thông báo hiện có
        // 2. Phân tích RecipientType và RecipientID để xác định loại chính xác
        // 3. Cập nhật SelectedRecipientType và tải danh sách đối tượng phù hợp
        private void SetRecipientTypeFromNotification(Notification notification)
        {
            if (notification.RecipientType == "All")
            {
                SelectedRecipientType = "All Users";
            }
            else if (notification.RecipientID.HasValue)
            {
                // If we have a specific recipient
                if (notification.RecipientType == "Student")
                {
                    SelectedRecipientType = "Specific Student";
                }
                else if (notification.RecipientType == "Teacher")
                {
                    SelectedRecipientType = "Specific Teacher";
                }
                else if (notification.RecipientType == "Class")
                {
                    SelectedRecipientType = "Specific Class";
                }
            }
            else
            {
                // If targeting all of a type
                SelectedRecipientType = "All " + notification.RecipientType + "s"; // Add 's' for plural
            }
        }
        
        // 1. Phương thức tải danh sách đối tượng nhận cụ thể
        // 2. Dựa vào loại đối tượng nhận được chọn
        // 3. Cập nhật danh sách Recipients và thiết lập đối tượng được chọn mặc định
        private async void LoadRecipientsAsync()
        {
            if (!ShowSpecificRecipient) return;
            
            try
            {
                IsProcessing = true;
                Recipients.Clear();
                
                if (SelectedRecipientType == "Specific Class")
                {
                    await LoadClassesAsync();
                }
                else if (SelectedRecipientType == "Specific Student")
                {
                    await LoadStudentsAsync();
                }
                else if (SelectedRecipientType == "Specific Teacher")
                {
                    await LoadTeachersAsync();
                }
                
                // Set selected recipient if in edit mode
                if (_isEditMode && _currentNotification != null && _currentNotification.RecipientID.HasValue)
                {
                    foreach (var recipient in Recipients)
                    {
                        if (recipient.ID == _currentNotification.RecipientID.Value)
                        {
                            SelectedRecipient = recipient;
                            break;
                        }
                    }
                }
                else if (Recipients.Count > 0)
                {
                    SelectedRecipient = Recipients[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recipients: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }
        
        // 1. Phương thức tải danh sách lớp học
        // 2. Tải các lớp học đang hoạt động từ cơ sở dữ liệu
        // 3. Cập nhật danh sách Recipients với các lớp học
        private async Task LoadClassesAsync()
        {
            string query = "SELECT ClassID, ClassName FROM Classes WHERE IsActive = 1 ORDER BY ClassName";
            var result = await _databaseService.ExecuteQueryAsync(query);
            
            foreach (DataRow row in result.Rows)
            {
                Recipients.Add(new RecipientItem
                {
                    ID = Convert.ToInt32(row["ClassID"]),
                    DisplayName = row["ClassName"].ToString(),
                    Type = "Class"
                });
            }
        }
        
        // 1. Phương thức tải danh sách học sinh
        // 2. Tải các học sinh đang hoạt động từ cơ sở dữ liệu
        // 3. Cập nhật danh sách Recipients với các học sinh
        private async Task LoadStudentsAsync()
        {
            string query = @"
                SELECT s.StudentID, CONCAT(s.FirstName, ' ', s.LastName) AS StudentName 
                FROM Students s
                JOIN Users u ON s.UserID = u.UserID
                WHERE u.IsActive = 1
                ORDER BY s.LastName, s.FirstName";
            var result = await _databaseService.ExecuteQueryAsync(query);
            
            foreach (DataRow row in result.Rows)
            {
                Recipients.Add(new RecipientItem
                {
                    ID = Convert.ToInt32(row["StudentID"]),
                    DisplayName = row["StudentName"].ToString(),
                    Type = "Student"
                });
            }
        }
        
        // 1. Phương thức tải danh sách giáo viên
        // 2. Tải các giáo viên đang hoạt động từ cơ sở dữ liệu
        // 3. Cập nhật danh sách Recipients với các giáo viên
        private async Task LoadTeachersAsync()
        {
            string query = @"
                SELECT t.TeacherID, CONCAT(t.FirstName, ' ', t.LastName) AS TeacherName 
                FROM Teachers t
                JOIN Users u ON t.UserID = u.UserID
                WHERE u.IsActive = 1
                ORDER BY t.LastName, t.FirstName";
            var result = await _databaseService.ExecuteQueryAsync(query);
            
            foreach (DataRow row in result.Rows)
            {
                Recipients.Add(new RecipientItem
                {
                    ID = Convert.ToInt32(row["TeacherID"]),
                    DisplayName = row["TeacherName"].ToString(),
                    Type = "Teacher"
                });
            }
        }
        
        // 1. Phương thức xác thực tiêu đề
        // 2. Kiểm tra xem tiêu đề có hợp lệ không
        // 3. Cập nhật lỗi nếu tiêu đề không hợp lệ
        private void ValidateTitle()
        {
            TitleError = string.Empty;
            
            if (string.IsNullOrWhiteSpace(Title))
            {
                TitleError = "Title is required";
            }
            else if (Title.Length > 100)
            {
                TitleError = "Title cannot exceed 100 characters";
            }
            
            OnPropertyChanged(nameof(HasTitleError));
        }
        
        // 1. Phương thức xác thực nội dung
        // 2. Kiểm tra xem nội dung có hợp lệ không
        // 3. Cập nhật lỗi nếu nội dung không hợp lệ
        private void ValidateMessage()
        {
            MessageError = string.Empty;
            
            if (string.IsNullOrWhiteSpace(Message))
            {
                MessageError = "Message is required";
            }
            else if (Message.Length > 1000)
            {
                MessageError = "Message cannot exceed 1000 characters";
            }
            
            OnPropertyChanged(nameof(HasMessageError));
        }
        
        // 1. Phương thức xác thực loại đối tượng nhận
        // 2. Kiểm tra xem đã chọn loại đối tượng nhận chưa
        // 3. Cập nhật lỗi nếu chưa chọn loại đối tượng nhận
        private void ValidateRecipientType()
        {
            RecipientTypeError = string.IsNullOrEmpty(SelectedRecipientType) ? 
                "Please select a recipient type" : string.Empty;
            OnPropertyChanged(nameof(HasRecipientTypeError));
        }
        
        // 1. Phương thức xác thực ngày hết hạn
        // 2. Kiểm tra xem ngày hết hạn có hợp lệ không
        // 3. Cập nhật lỗi nếu ngày hết hạn không hợp lệ
        private void ValidateExpiryDate()
        {
            ExpiryDateError = string.Empty;
            
            if (ExpiryDate.HasValue)
            {
                // Validate time format
                TimeSpan timeValue;
                bool validTime = TimeSpan.TryParseExact(ExpiryTime, "hh\\:mm", CultureInfo.InvariantCulture, out timeValue);
                
                if (!validTime)
                {
                    ExpiryDateError = "Invalid time format. Use HH:mm";
                }
                else
                {
                    // Combine date and time
                    DateTime combinedDateTime = ExpiryDate.Value.Date.Add(timeValue);
                    
                    // Check if date is in the past
                    if (combinedDateTime <= DateTime.Now)
                    {
                        ExpiryDateError = "Expiry date must be in the future";
                    }
                }
            }
            
            OnPropertyChanged(nameof(HasExpiryDateError));
        }
        
        // 1. Phương thức kiểm tra xem có thể lưu thông báo không
        // 2. Kiểm tra tất cả các trường bắt buộc đã được nhập
        // 3. Trả về true nếu tất cả các trường hợp lệ, false nếu không
        private bool CanSaveNotification()
        {
            bool isValid = !string.IsNullOrWhiteSpace(Title) &&
                          !string.IsNullOrWhiteSpace(Message) &&
                          !string.IsNullOrEmpty(SelectedRecipientType) &&
                          !HasTitleError &&
                          !HasMessageError &&
                          !HasRecipientTypeError &&
                          !HasExpiryDateError;
            
            // Check that we have a selected recipient if needed
            if (ShowSpecificRecipient)
            {
                isValid = isValid && SelectedRecipient != null;
            }
            
            return isValid;
        }
        
        // 1. Phương thức lưu thông báo
        // 2. Xác thực dữ liệu trước khi lưu
        // 3. Thêm mới hoặc cập nhật thông báo tùy thuộc vào IsEditMode
        private async Task SaveNotificationAsync()
        {
            // Validate all fields
            ValidateTitle();
            ValidateMessage();
            ValidateRecipientType();
            ValidateExpiryDate();
            
            if (!CanSaveNotification()) return;
            
            IsProcessing = true;
            
            try
            {
                // Determine actual recipient type and ID
                string recipientType;
                int? recipientId = null;
                
                if (SelectedRecipientType == "All Users")
                {
                    recipientType = "All";
                }
                else if (SelectedRecipientType.StartsWith("All "))
                {
                    // Remove "All " and trim the 's' from the end
                    recipientType = SelectedRecipientType.Substring(4, SelectedRecipientType.Length - 5);
                }
                else if (ShowSpecificRecipient && SelectedRecipient != null)
                {
                    recipientType = SelectedRecipient.Type;
                    recipientId = SelectedRecipient.ID;
                }
                else
                {
                    throw new InvalidOperationException("Invalid recipient selection");
                }
                
                // Determine expiry date
                DateTime? expiryDateTime = null;
                if (ExpiryDate.HasValue)
                {
                    TimeSpan timeValue;
                    if (TimeSpan.TryParseExact(ExpiryTime, "hh\\:mm", CultureInfo.InvariantCulture, out timeValue))
                    {
                        expiryDateTime = ExpiryDate.Value.Date.Add(timeValue);
                    }
                    else
                    {
                        // Default to 23:59 if time is invalid
                        expiryDateTime = ExpiryDate.Value.Date.AddHours(23).AddMinutes(59);
                    }
                }
                
                if (_isEditMode)
                {
                    // Update existing notification
                    await UpdateNotificationAsync(recipientType, recipientId, expiryDateTime);
                }
                else
                {
                    // Create new notification
                    await CreateNotificationAsync(recipientType, recipientId, expiryDateTime);
                }
                
                CloseDialog(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving notification: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }
        
        // 1. Phương thức tạo thông báo mới
        // 2. Thêm mới thông báo vào cơ sở dữ liệu
        // 3. Sử dụng các tham số đã được xác thực
        private async Task CreateNotificationAsync(string recipientType, int? recipientId, DateTime? expiryDateTime)
        {
            string query = @"
                INSERT INTO Notifications 
                    (Title, Message, SenderID, RecipientType, RecipientID, IsRead, CreatedDate, ExpiryDate)
                VALUES 
                    (@Title, @Message, @SenderID, @RecipientType, @RecipientID, @IsRead, @CreatedDate, @ExpiryDate)";
            
            var parameters = new Dictionary<string, object>
            {
                { "@Title", Title },
                { "@Message", Message },
                { "@SenderID", _currentUser.UserID },
                { "@RecipientType", recipientType },
                { "@IsRead", false }
            };
            
            // Add nullable parameters
            parameters.Add("@RecipientID", recipientId.HasValue ? (object)recipientId.Value : DBNull.Value);
            parameters.Add("@CreatedDate", DateTime.Now);
            parameters.Add("@ExpiryDate", expiryDateTime.HasValue ? (object)expiryDateTime.Value : DBNull.Value);
            
            await _databaseService.ExecuteNonQueryAsync(query, parameters);
        }
        
        // 1. Phương thức cập nhật thông báo
        // 2. Cập nhật thông báo trong cơ sở dữ liệu
        // 3. Sử dụng các tham số đã được xác thực
        private async Task UpdateNotificationAsync(string recipientType, int? recipientId, DateTime? expiryDateTime)
        {
            string query = @"
                UPDATE Notifications 
                SET Title = @Title, 
                    Message = @Message, 
                    RecipientType = @RecipientType, 
                    RecipientID = @RecipientID, 
                    IsRead = @IsRead, 
                    ExpiryDate = @ExpiryDate
                WHERE NotificationID = @NotificationID";
            
            var parameters = new Dictionary<string, object>
            {
                { "@NotificationID", _currentNotification.NotificationID },
                { "@Title", Title },
                { "@Message", Message },
                { "@RecipientType", recipientType },
                { "@IsRead", IsRead }
            };
            
            // Add nullable parameters
            parameters.Add("@RecipientID", recipientId.HasValue ? (object)recipientId.Value : DBNull.Value);
            parameters.Add("@ExpiryDate", expiryDateTime.HasValue ? (object)expiryDateTime.Value : DBNull.Value);
            
            await _databaseService.ExecuteNonQueryAsync(query, parameters);
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
    
    // Lớp RecipientItem đơn giản hóa để sử dụng trong ComboBox
    // + Tại sao cần sử dụng: Lưu trữ thông tin cơ bản của đối tượng nhận để hiển thị trong ComboBox
    // + Lớp này chỉ chứa các thuộc tính cần thiết cho việc hiển thị và xử lý trong AddEditNotificationViewModel
    // + Chức năng chính: Cung cấp cấu trúc dữ liệu đơn giản cho đối tượng nhận thông báo
    public class RecipientItem
    {
        // 1. ID của đối tượng nhận
        // 2. Có thể là ClassID, StudentID hoặc TeacherID tùy theo Type
        // 3. Được sử dụng khi lưu thông báo
        public int ID { get; set; }
        
        // 1. Tên hiển thị của đối tượng nhận
        // 2. Hiển thị trong ComboBox
        // 3. Dùng để người dùng dễ dàng chọn đối tượng nhận
        public string DisplayName { get; set; } = string.Empty;
        
        // 1. Loại đối tượng nhận
        // 2. Có thể là "Class", "Student" hoặc "Teacher"
        // 3. Dùng để xác định loại ID khi lưu thông báo
        public string Type { get; set; } = string.Empty;
    }
}
