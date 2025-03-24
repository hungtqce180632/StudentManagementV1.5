using System.Configuration;
using System.Data;
using System.Windows;
using StudentManagementV1._5.Services;
using StudentManagementV1._5.ViewModels;
using StudentManagementV1._5.Views;

namespace StudentManagementV1._5;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
    }
}

