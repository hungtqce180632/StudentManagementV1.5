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
    public class NotificationManagementViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly NavigationService _navigationService;
        private readonly AuthenticationService _authService;

        private ObservableCollection<Notification> _notifications = [];
        private string _searchText = string.Empty;
        private bool _showExpired = false;
        private bool _isLoading;
        private Notification? _selectedNotification;
        private string _selectedRecipientTypeFilter = "All";
        private ObservableCollection<string> _recipientTypeFilters = new ObservableCollection<string>
        {
            "All", "All Users", "Class", "Teacher", "Student"
        };

        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }

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

        public ObservableCollection<string> RecipientTypeFilters
        {
            get => _recipientTypeFilters;
            set => SetProperty(ref _recipientTypeFilters, value);
        }

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

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public Notification? SelectedNotification
        {
            get => _selectedNotification;
            set => SetProperty(ref _selectedNotification, value);
        }

        public ICommand CreateNotificationCommand { get; }
        public ICommand ViewNotificationCommand { get; }
        public ICommand EditNotificationCommand { get; }
        public ICommand DeleteNotificationCommand { get; }
        public ICommand BackCommand { get; }

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
        
        private async Task RefreshNotificationsAsync()
        {
            await LoadNotificationsAsync();
        }
        
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
        
        private void CreateNotification()
        {
            // For now just show a placeholder message
            // In a future update, implement a Create Notification dialog
            MessageBox.Show("Create Notification functionality will be implemented in a future update.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
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
        
        private void EditNotification(Notification? notification)
        {
            if (notification == null) return;
            
            // For now just show a placeholder message
            // In a future update, implement an Edit Notification dialog
            MessageBox.Show($"Edit Notification '{notification.Title}' functionality will be implemented in a future update.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
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
