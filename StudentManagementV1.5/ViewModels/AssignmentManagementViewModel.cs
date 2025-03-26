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
    // Lớp AssignmentManagementViewModel
    // + Tại sao cần sử dụng: Quản lý và hiển thị danh sách bài tập giáo viên đã giao
    // + Lớp này được gọi từ màn hình quản lý bài tập của giáo viên
    // + Chức năng chính: Thêm, xóa, chỉnh sửa bài tập và quản lý nộp bài
    public class AssignmentManagementViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly NavigationService _navigationService;
        private readonly AuthenticationService _authService;
        
        private string _errorMessage = string.Empty;
        private bool _isLoading;

        // Properties
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Commands
        public ICommand BackCommand { get; }

        // Constructor
        public AssignmentManagementViewModel(DatabaseService databaseService, NavigationService navigationService, AuthenticationService authService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _authService = authService;

            // Initialize commands
            BackCommand = new RelayCommand(_ => _navigationService.NavigateTo(AppViews.TeacherDashboard));

            // Show implementation message
            MessageBox.Show("Assignment Management feature will be implemented in a future update.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
