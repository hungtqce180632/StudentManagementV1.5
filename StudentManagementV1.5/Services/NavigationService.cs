using System;
using System.Windows.Controls;
using StudentManagementV1._5.ViewModels;

namespace StudentManagementV1._5.Services
{
    // Lớp NavigationService
    // + Tại sao cần sử dụng: Quản lý việc điều hướng giữa các màn hình trong ứng dụng
    // + Lớp này được gọi từ các ViewModel khi cần chuyển đổi màn hình
    // + Chức năng chính: Điều hướng qua lại giữa các màn hình và quản lý lịch sử điều hướng
    public class NavigationService
    {
        // 1. Frame chứa nội dung hiển thị
        // 2. Được sử dụng để nạp và hiển thị các màn hình
        // 3. Thường là frame chính của ứng dụng
        private readonly Frame _navigationFrame;
        
        // 1. Hàm để tạo UserControl từ loại màn hình
        // 2. Được truyền vào từ MainWindow
        // 3. Giúp chuyển đổi từ enum AppViews sang UserControl tương ứng
        private readonly Func<AppViews, UserControl> _pageResolver;

        // Function to resolve views with parameters
        private readonly Func<AppViews, object, UserControl> _pageResolverWithParameter;
        
        // Object parameter to pass to the next view
        private object _parameter;
        
        // Authentication service for use with MyCoursesViewModel
        private readonly AuthenticationService _authService;

        // Add a new event to notify MainWindow when logout occurs
        public event EventHandler LogoutRequested;

        // 1. Constructor của NavigationService
        // 2. Khởi tạo service với frame và hàm resolver
        // 3. Được gọi khi ứng dụng khởi động
        public NavigationService(Frame navigationFrame, Func<AppViews, UserControl> pageResolver, 
            AuthenticationService authService,
            Func<AppViews, object, UserControl> pageResolverWithParameter = null)
        {
            _navigationFrame = navigationFrame;
            _pageResolver = pageResolver;
            _pageResolverWithParameter = pageResolverWithParameter;
            _authService = authService;
        }

        // 1. Phương thức điều hướng đến màn hình mới
        // 2. Nhận enum AppViews và chuyển thành UserControl
        // 3. Nạp UserControl vào frame để hiển thị
        public void NavigateTo(AppViews view)
        {
            // Special handling for Login view to handle the logout scenario
            if (view == AppViews.Login)
            {
                // Raise the event to notify MainWindow to handle the logout process
                LogoutRequested?.Invoke(this, EventArgs.Empty);
                return;
            }
            
            UserControl page;
            
            // Special handling for the MyCourses view since it requires special initialization
            if (view == AppViews.MyCourses)
            {
                var viewModel = new MyCoursesViewModel(_authService, this);
                page = new Views.MyCoursesView { DataContext = viewModel };
            }
            else
            {
                page = _pageResolver(view);
            }
            
            _navigationFrame.Navigate(page);
        }

        // 1. Phương thức quay lại màn hình trước đó
        // 2. Kiểm tra xem có thể quay lại không
        // 3. Nếu có thể, thực hiện quay lại
        public void GoBack()
        {
            if (_navigationFrame.CanGoBack)
            {
                _navigationFrame.GoBack();
            }
        }

        // 1. Phương thức điều hướng đến màn hình mới với tham số
        // 2. Nhận enum AppViews và object parameter, chuyển thành UserControl
        // 3. Lưu tham số để truyền vào view cần hiển thị
        public void NavigateToWithParameter(AppViews view, object parameter)
        {
            _parameter = parameter;
            
            UserControl page;
            if (_pageResolverWithParameter != null)
            {
                page = _pageResolverWithParameter(view, parameter);
            }
            else
            {
                // Fall back to regular resolver if parameter resolver is not provided
                page = _pageResolver(view);
            }
            
            _navigationFrame.Navigate(page);
        }
        
        // 1. Phương thức lấy tham số đã được truyền vào
        // 2. Được gọi từ view sau khi điều hướng
        // 3. Trả về tham số và xóa nó
        public object GetAndClearParameter()
        {
            var param = _parameter;
            _parameter = null;
            return param;
        }
    }
}