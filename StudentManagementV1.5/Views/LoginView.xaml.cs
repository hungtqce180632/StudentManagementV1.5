using StudentManagementV1._5.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Add this missing namespace for CommandManager

namespace StudentManagementV1._5.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            // Connect to loaded event to set up password binding
            Loaded += LoginView_Loaded;
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                // Handle password box changes
                if (PasswordBox != null)
                {
                    PasswordBox.PasswordChanged += (s, args) =>
                    {
                        viewModel.Password = PasswordBox.Password;
                    };
                }
                
                // Also ensure command can execute is checked
                viewModel.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(LoginViewModel.Password) ||
                        args.PropertyName == nameof(LoginViewModel.Username))
                    {
                        CommandManager.InvalidateRequerySuggested();
                    }
                };
            }
        }
    }
}
