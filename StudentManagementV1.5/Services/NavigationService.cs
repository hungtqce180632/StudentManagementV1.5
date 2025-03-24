using System;
using System.Windows.Controls;

namespace StudentManagementV1._5.Services
{
    public enum AppViews
    {
        // Basic views
        Login,
        PasswordReset,
        
        // Dashboard views
        AdminDashboard,
        TeacherDashboard,
        StudentDashboard,
        
        // Admin management views
        UserManagement,
        SubjectManagement,
        ExamManagement,
        ScoreManagement,
        ClassManagement,
        ScheduleManagement,
        NotificationManagement,
        
        // Teacher views
        AssignmentManagement,
        
        // Student views
        SubmissionManagement,
        ViewAssignments
    }

    public class NavigationService
    {
        private readonly Frame _navigationFrame;
        private readonly Func<AppViews, UserControl> _pageResolver;

        public NavigationService(Frame navigationFrame, Func<AppViews, UserControl> pageResolver)
        {
            _navigationFrame = navigationFrame;
            _pageResolver = pageResolver;
        }

        public void NavigateTo(AppViews view)
        {
            var page = _pageResolver(view);
            _navigationFrame.Navigate(page);
        }

        public void GoBack()
        {
            if (_navigationFrame.CanGoBack)
            {
                _navigationFrame.GoBack();
            }
        }
    }
}