using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StudentManagementV1._5.Commands;
using StudentManagementV1._5.Services;
using StudentManagementV1._5.ViewModels;
using StudentManagementV1._5.Views;

namespace StudentManagementV1._5;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;
    private readonly AuthenticationService _authService;
    private readonly EmailService _emailService;
    private readonly Services.NavigationService _navigationService;
    private bool _isLoginVisible = true;
    private LoginViewModel _loginViewModel;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsLoginVisible
    {
        get => _isLoginVisible;
        set
        {
            _isLoginVisible = value;
            OnPropertyChanged();
        }
    }

    public MainWindow()
    {
        // Set DataContext to this instance for bindings
        DataContext = this;
        
        InitializeComponent();

        // Initialize services
        _databaseService = new DatabaseService();
        _authService = new AuthenticationService(_databaseService);
        _emailService = new EmailService();
        
        // Setup navigation
        _navigationService = new Services.NavigationService(MainFrame, ResolveView);
        
        // Setup login view
        _loginViewModel = new LoginViewModel(_authService, _navigationService);
        _loginViewModel.LoginSuccessful += OnLoginSuccessful;
        ShowLoginView();
    }

    private void ShowLoginView()
    {
        // Create the login view and set its DataContext
        var loginView = new LoginView();
        loginView.DataContext = _loginViewModel;
        
        // Set the login view as the content of the main window
        MainContent.Content = loginView;
    }

    private void OnLoginSuccessful(object sender, EventArgs e)
    {
        // Hide login view and show main content
        IsLoginVisible = false;
        
        // Navigate to the appropriate dashboard based on user role
        switch (_authService.CurrentUser?.Role)
        {
            case "Admin":
                _navigationService.NavigateTo(AppViews.AdminDashboard);
                break;
            case "Teacher":
                _navigationService.NavigateTo(AppViews.TeacherDashboard);
                break;
            case "Student":
                _navigationService.NavigateTo(AppViews.StudentDashboard);
                break;
            default:
                MessageBox.Show("Unknown user role", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                break;
        }
    }

    private UserControl ResolveView(AppViews view)
    {
        return view switch
        {
            // Basic views
            AppViews.Login => new LoginView { DataContext = new LoginViewModel(_authService, _navigationService) },
            AppViews.PasswordReset => new PasswordResetView { DataContext = new PasswordResetViewModel(_authService, _emailService, _navigationService) },
            
            // Dashboard views
            AppViews.AdminDashboard => new AdminDashboardView { DataContext = new AdminDashboardViewModel(_authService, _navigationService) },
            AppViews.TeacherDashboard => new TeacherDashboardView { DataContext = new TeacherDashboardViewModel(_authService, _navigationService) },
            AppViews.StudentDashboard => new StudentDashboardView { DataContext = new StudentDashboardViewModel(_authService, _navigationService) },
            
            // Admin management views
            AppViews.UserManagement => new UserManagementView { DataContext = new UserManagementViewModel(_databaseService, _authService, _navigationService) },
            
            // Add the rest of the views as you implement them
            _ => throw new System.NotImplementedException($"View {view} is not implemented")
        };
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}